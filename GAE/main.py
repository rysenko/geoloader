# -*- coding: utf-8 -*-
from __future__ import with_statement
from google.appengine.ext import webapp
from google.appengine.api import files
from google.appengine.ext import blobstore
from google.appengine.ext.webapp import util
from google.appengine.ext.webapp import template
from google.appengine.api import taskqueue
from django.utils import simplejson
from datetime import datetime, timedelta
import os
import re
import tools

class Main(webapp.RequestHandler):
  def get(self):
    content = tools.Client().get('http://pda.geocaching.su/list.php')
    countries = re.findall('<a href="list.php\?c=(\d+)">([^<]+)</a>', content)
    template_values = {
      'countries': countries,
    }
    path = os.path.join(os.path.dirname(__file__), 'main.html')
    self.response.out.write(template.render(path, template_values))

class Regions(webapp.RequestHandler):
  def get(self):
    self.response.headers['Content-Type'] = 'text/json; charset=utf-8'
    content = tools.Client().get('http://pda.geocaching.su/list.php?c=' + self.request.get('country'))
    regions = re.findall('<a href="list.php\?c=\d+&a=(\d+)">([^<]+)</a>', content)
    self.response.out.write(simplejson.dumps(regions))

class Status(webapp.RequestHandler):
  def post(self):
    taskqueue.add(url='/worker', params={'country': self.request.get('country'), 'region': self.request.get('region'), 'file_name': self.request.get('file_name')})
    template_values = {
      'file_name': self.request.get('file_name'),
    }
    path = os.path.join(os.path.dirname(__file__), 'status.html')
    self.response.out.write(template.render(path, template_values))

  def get(self):
    file_url = self.request.get('file_name')
    blobs = blobstore.BlobInfo.gql("WHERE filename='%s' LIMIT 1" % file_url)
    if blobs.count() > 0:
      blob = blobs[0]
      if (datetime.now() - blob.creation) < timedelta (days = 3):
        self.response.out.write('ready')
        return
    self.response.out.write('working')


class Worker(webapp.RequestHandler):
  def post(self):
    file_url = self.request.get('file_name')
    blobs = blobstore.BlobInfo.gql("WHERE filename='%s' LIMIT 1" % file_url)
    if blobs.count() > 0:
      blob = blobs[0]
      if (datetime.now() - blob.creation) > timedelta (days = 3):
        blobstore.BlobInfo.delete(blobs[0])
      else:
        return
    caches_all = []
    skip = 0
    while True:
      url = 'http://pda.geocaching.su/list.php?c=%s&a=%s&skip=%d' % (self.request.get('country'), self.request.get('region'), skip);
      caches_data = tools.Client().get(url)
      caches = re.findall('<a href="cache.php\?cid=(\d+)"><b>[^<]+</b></a>', caches_data)
      if len(caches) < 1:
        break
      caches_all.extend(caches)
      skip += 20
    caches = []
    for cache_id in caches_all:
      cache = tools.CacheLoader(cache_id).load()
      caches.append(cache)
    file_name = files.blobstore.create(mime_type='text/xml',_blobinfo_uploaded_filename=file_url)
    datastr = tools.CacheSaver().save_all(caches)
    with files.open(file_name, 'a') as stream:
      while len(datastr) > 0:
        stream.write(datastr[0:65536])
        datastr = datastr[65536:]
    files.finalize(file_name)

  def get(self):
    self.post()

class Download(webapp.RequestHandler):
  def get(self):
    url = self.request.get('file_name')
    blobs = blobstore.BlobInfo.gql("WHERE filename='%s' LIMIT 1" % url)
    if blobs.count() > 0:
      blob = blobs[0]
      blob_reader = blobstore.BlobReader(blob.key())
      self.response.headers['Content-Type'] = 'text/xml; charset=utf-8'
      self.response.headers['Content-Disposition'] = 'attachment; filename=' + url
      self.response.out.write(blob_reader.read())

def main():
  application = webapp.WSGIApplication([('/', Main), ('/regions', Regions), ('/status', Status), ('/worker', Worker), ('/download', Download)], debug=True)
  util.run_wsgi_app(application)

if __name__ == '__main__':
  main()
