# PersonalWebSMS Backend System
# Get the SMS data from web frontend service, and then pass it into Gammu SMS Gateway for further process
#
# Data are taken from ASP.NET MVC WebApi using JSON data structures
# Data sent to Gammu SMS Gateway via MySQL Database

import json, urllib, urllib2
import _mysql, time
import datetime


def process():
	# Request Process
	req_url = 'http://10.151.34.36/KELAS_C/KLP_01/Message/SMSAPI'
	req_params = urllib.urlencode(dict(APIId='hanahbanana', APISecretCode='segogoreng'))
	request = urllib2.urlopen(req_url, req_params)
	response = request.readline()

	# JSON Decode
	json_data = json.loads(response)

	# Connecting to MYSQL Server
	mysql_con = _mysql.connect('127.0.0.1', 'root', '', 'gammu')

	# Iterating data and entry to MYSQL Gammu server
	for i in json_data:
		sms_dest = i['Dest']
		sms_msg = i['Msg']
		print sms_dest + ": " + sms_msg
		mysql_con.query("INSERT INTO outbox(DestinationNumber, TextDecoded) VALUES ('" + sms_dest + "','" + sms_msg + "')")

	server_string = "Genuine Windows 8 SL Edition - HP Envy dv4 - Gammu version 1.32"
	last_update = datetime.datetime.now()
	req_url = 'http://10.151.34.36/KELAS_C/KLP_01/Report/ReportAPI'
	req_params = urllib.urlencode(dict(APIId='hahaha', APISecretCode='hihihi', ServerString=server_string, LastUpdate=last_update))
	request = urllib2.urlopen(req_url, req_params)
	response = request.readline()

	# Close MySQL Connection
	if mysql_con:
		mysql_con.close()
	return

while(1):
	process()