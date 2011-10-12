# -*- coding: utf-8 -*-
from __future__ import with_statement
from datetime import datetime, timedelta
from google.appengine.api import files
from google.appengine.ext import blobstore
from xml.etree import ElementTree as ET
from htmlentitydefs import name2codepoint
import urllib
import re

class Client:
  def get(self, url, expire_days=3):
    blobs = blobstore.BlobInfo.gql("WHERE filename='%s' LIMIT 1" % url)
    if blobs.count() > 0:
      blob = blobs[0]
      if (datetime.now() - blob.creation) > timedelta (days = expire_days):
        blobstore.BlobInfo.delete(blobs[0])
      else:
        blob_reader = blobstore.BlobReader(blob.key())
        return unicode(blob_reader.read(), 'windows-1251')
    binary = urllib.urlopen(url).read()
    file_name = files.blobstore.create(mime_type='text/html',_blobinfo_uploaded_filename=url)
    with files.open(file_name, 'a') as stream:
      stream.write(binary)
    files.finalize(file_name)
    return unicode(binary, 'windows-1251')

class CacheLoader:
  def __init__(self, cache_id):
    self.cache_id = cache_id

  def load(self):
    cache = {'id': self.cache_id}
    self.cache_data = Client().get('http://pda.geocaching.su/cache.php?cid=' + self.cache_id, 15)
    cache['country'] = self.get_field_value(u'Страна', True)
    cache['region'] = self.get_field_value(u'Область', False)
    cache['difficulty'] = self.get_field_value(u'Доступность', True)
    cache['terrain'] = self.get_field_value(u'Местность', True)
    coordinates = self.get_field_value(u'Координаты \(WGS 84\)', True).replace('<font class=coords>', '').replace('</font>', '');
    coordinates_match = re.search('([NS]) (\d{1,2})&#176; (\d{1,2}.\d{3})\' &nbsp;&nbsp;&nbsp;([EW]) (\d{1,3})&#176; (\d{1,2}.\d{3})\'', coordinates)
    if not coordinates_match:
      raise Exception(u'Ошибка парсинга координат для кэша ' + self.cache_id)
    cache['latitude'] = int(coordinates_match.group(2)) + float(coordinates_match.group(3)) / 60
    if coordinates_match.group(1) == 'S':
      cache['latitude'] = -cache['latitude']
    cache['longitude'] = int(coordinates_match.group(5)) + float(coordinates_match.group(6)) / 60
    if coordinates_match.group(4) == 'W':
      cache['longitude'] = -cache['longitude']
    cache['short_description'] = self.get_block_value(u'Атрибуты', False)
    cache['long_description'] = self.get_block_value(u'Описание окружающей местности', True)
    if len(cache['long_description']) > 8192:
      cache['long_description'] = cache['long_description'][0:8189] + '...'
    cache['contents'] = self.get_block_value(u'Содержимое тайника', False)
    cache['hints'] = self.get_block_value(u'Описание тайника', False)
    name_match = re.search(u'<p><b>([^<]+)</b> от <b><a href="profile.php\?uid=(\d+)">\'(.+?)\'</a></b>(?:<br>|\s+)<i>\([^<]+? (\w{2})' + self.cache_id + '\)</i>', self.cache_data)
    if not name_match:
      raise Exception(u'Ошибка парсинга имени и автора для кэша ' + self.cache_id)
    cache['name'] = name_match.group(1)
    cache['author_id'] = name_match.group(2)
    cache['author_name'] = name_match.group(3)
    cache['type_code'] = name_match.group(4)
    cache['type'] = 'Unknown Cache'
    if cache['type_code'] == 'TR':
      cache['type'] = 'Traditional Cache'
    elif cache['type_code'] == 'MS':
      cache['type'] = 'Multi-cache'
    elif cache['type_code'] == 'VI':
      cache['type'] = 'Virtual Cache'
    created = self.get_field_value(u'Создан', True)
    cache['author_date'] = datetime.strptime(created, '%d.%m.%Y')
    log_data = Client().get('http://pda.geocaching.su/note.php?cid=' + self.cache_id)
    log_match = re.findall(u'<b><u>([^<]+)</u></b><i> от ([0-9\.]+)</i><br> ([\w\W]+?)<br>(<p>)?<hr>', log_data)
    logs_added = 0
    cache['log'] = []
    for log_entry in log_match:
      if logs_added < 5:
        cache['log'].append({'author_name': log_entry[0], 'text': log_entry[2], 'author_date': datetime.strptime(log_entry[1], u'%d.%m.%Y')})
      logs_added += 1
    return cache

  def get_field_value(self, field_name, required):
    match_result = re.search(field_name + ': <b>(.+?)</b>', self.cache_data)
    if match_result:
      return match_result.group(1)
    elif required:
      raise Exception(u'Поле' + field_name + u' не найдено для кэша ' + self.cache_id)
    return ''

  def get_block_value(self, block_name, required):
    match_result = re.search('<b><u>' + block_name + '</u></b>(<br>)?([\w\W]+?)<p><hr>', self.cache_data)
    if match_result:
      return match_result.group(2)
    elif required:
      raise Exception(u'Блок ' + block_name + u' не найден для кэша ' + self.cache_id)
    return ''

class CacheSaver:
  entitydefs = None
  def html_decode(self, s):
    if '&' not in s:
      return s
    def replace_entities(s):
      s = s.groups()[0]
      try:
        if s[0] == '#':
          s = s[1:]
          if s[0] in ['x','X']:
            c = int(s[1:], 16)
          else:
            c = int(s)
          return unichr(c)
      except ValueError:
        return '&#'+s+';'
      else:
        import htmlentitydefs
        if CacheSaver.entitydefs is None:
          entitydefs = CacheSaver.entitydefs = {'apos':u"'"}
          for k, v in htmlentitydefs.name2codepoint.iteritems():
            entitydefs[k] = unichr(v)
        try:
          return self.entitydefs[s]
        except KeyError:
          return '&'+s+';'
    s = re.sub(r'&(#?[xX]?(?:[0-9a-fA-F]+|\w{1,8}));', replace_entities, s)
    return s

  def html_to_text(self, s):
     s = self.html_decode(s)
     s = s.replace('\n', ' ')
     s = s.replace('\t', ' ')
     s = re.sub('\s+', ' ', s)
     s = s.replace('<br>', '\n<br>')
     s = s.replace('<p>', '\n<p>')
     s = s.replace('<br ', '\n<br ')
     s = s.replace('<p ', '\n<p ')
     s = re.sub('<[^>]*>', '', s)
     return s.strip()

  def save(self, cache):
    point = ET.SubElement(self.root, 'wpt')
    point.set('lat', str(cache['latitude']))
    point.set('lon', str(cache['longitude']))
    ET.SubElement(point, 'time').text = cache['author_date'].isoformat()
    ET.SubElement(point, 'name').text = cache['type_code'] + cache['id']
    ET.SubElement(point, 'desc').text = cache['name']
    ET.SubElement(point, 'url').text = 'http://pda.geocaching.su/cache.php?cid=' + cache['id']
    ET.SubElement(point, 'urlname').text = cache['name']
    ET.SubElement(point, 'sym').text = 'Geocache'
    ET.SubElement(point, 'type').text = 'Geocache|' + cache['type']
    groundspeak = ET.SubElement(point, 'cache')
    groundspeak.set('xmlns', 'http://www.groundspeak.com/cache/1/0')
    groundspeak.set('id', cache['id'])
    groundspeak.set('available', 'True')
    groundspeak.set('archived', 'False')
    ET.SubElement(groundspeak, 'type').text = cache['type']
    ET.SubElement(groundspeak, 'name').text = cache['name']
    ET.SubElement(groundspeak, 'placed_by').text = cache['author_name']
    owner = ET.SubElement(groundspeak, 'owner')
    owner.set('id', cache['author_id'])
    owner.text = cache['author_name']
    ET.SubElement(groundspeak, 'country').text = cache['country']
    if not cache['region'] == '':
      ET.SubElement(groundspeak, 'state').text = cache['region']
    ET.SubElement(groundspeak, 'difficulty').text = str(cache['difficulty'])
    ET.SubElement(groundspeak, 'terrain').text = str(cache['terrain'])
    short_description = ET.SubElement(groundspeak, 'short_description')
    short_description.set('html', 'True')
    short_description.text = cache['short_description']
    long_description = ET.SubElement(groundspeak, 'long_description')
    long_description.set('html', 'True')
    long_description.text = self.html_decode(cache['long_description'])
    hints = ET.SubElement(groundspeak, 'encoded_hints')
    hints.set('html', 'True')
    hints.text = self.html_to_text(cache['hints'])
    logs = ET.SubElement(groundspeak, 'logs')
    for log_entry in cache['log']:
      log = ET.SubElement(logs, 'log')
      ET.SubElement(log, 'type').text = 'Found it'
      ET.SubElement(log, 'date').text = log_entry['author_date'].isoformat()
      ET.SubElement(log, 'finder').text = log_entry['author_name']
      ET.SubElement(log, 'text').text = log_entry['text']

  def save_all(self, caches):
    self.root = ET.Element('gpx')
    self.root.set('xmlns', 'http://www.topografix.com/GPX/1/0')
    self.root.set('version', '1.0')
    self.root.set('creator', 'http://rugeoloader.appspot.com/');
    self.root.set('xmlns:xsi', 'http://www.w3.org/2001/XMLSchema-instance')
    self.root.set('xmlns:xsd', 'http://www.w3.org/2001/XMLSchema')
    self.root.set('xsi:schemaLocation', 'http://www.topografix.com/GPX/1/0 http://www.topografix.com/GPX/1/0/gpx.xsd http://www.groundspeak.com/cache/1/0 http://www.groundspeak.com/cache/1/0/cache.xsd')
    ET.SubElement(self.root, 'name').text = 'Cache Listing Generated from Geocaching.Ru'
    ET.SubElement(self.root, 'desc').text = 'This is a cache list generated from Geocaching.Ru'
    ET.SubElement(self.root, 'email').text = 'org@geocaching.ru'
    ET.SubElement(self.root, 'url').text = 'http://www.geocaching.su'
    ET.SubElement(self.root, 'urlname').text = 'Geocaching - High Tech Treasure Hunting'
    ET.SubElement(self.root, 'keywords').text = 'cache, geocache'
    bounds = ET.SubElement(self.root, 'bounds')
    bounds.set('minlat', '-90')
    bounds.set('minlon', '0')
    bounds.set('maxlat', '90')
    bounds.set('maxlon', '180')
    for cache in caches:
      self.save(cache)
    return ET.tostring(self.root, 'utf-8')