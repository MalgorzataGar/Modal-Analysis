<<<<<<< HEAD
#include <ESP8266WiFi.h>
#include <ArduinoJson.h>
#include <WiFiClient.h>
#include <ESP8266WebServer.h>
#include <SoftwareSerial.h>
#include <Wire.h>
#include <LSM303.h>
#include <FS.h>

LSM303 bar;
LSM303 hammer;

//char* filename;
String filename;
const char *ssid = "nazwa wifi";
WiFiServer server(80);

String header;
unsigned long startTime;
unsigned long timeS;
unsigned long currentTime;
unsigned long previousTime = 0;
const long timeoutTime = 2000;

void setup() {
  Serial.begin(115200);
=======
#include <ArduinoJson.h>
#include <ESP8266WiFi.h>
#include <WiFiClient.h>
#include <ESP8266WebServer.h>
#include <SoftwareSerial.h>


const char *ssid = "MikroTik-CF2A6D";
ESP8266WebServer server(80);

String header;

// Current time
unsigned long currentTime = millis();
// Previous time
unsigned long previousTime = 0;
// Define timeout time in milliseconds
const long timeoutTime = 2000;

struct ConfigData {
  String Mode;
  String ADR;
  String App_key;
  String Dev_addr;
  String Join;
  int DR;
};

ConfigData configData;
void setup() {
  Serial.begin(9600);
>>>>>>> 4b5e463ee9eb6c82b6551839dec9661d5e5fb597
  Serial.print("Connecting to ");
  Serial.println(ssid);
  WiFi.begin(ssid);
  while (WiFi.status() != WL_CONNECTED)
  {
    delay(500);
    Serial.print(".");
  }
  Serial.println("");
  Serial.println("WiFi connected.");
  Serial.println("IP address: ");
  Serial.println(WiFi.localIP());
  server.begin();
<<<<<<< HEAD
  while(!Serial) { delay(100); }
    Wire.begin();
 if(SPIFFS.begin())
  {
    Serial.println("SPIFFS Initialize....ok");
  }
  else
  {
    Serial.println("SPIFFS Initialization...failed");
  }
 
  //Format File System
  if(SPIFFS.format())
  {
    Serial.println("File System Formated");
  }
  else
  {
    Serial.println("File System Formatting Error");
  }
  bar.init(LSM303::device_auto, LSM303::sa0_low);
  hammer.init(LSM303::device_auto, LSM303::sa0_high);
  bar.enableDefault();
  hammer.enableDefault();
=======


>>>>>>> 4b5e463ee9eb6c82b6551839dec9661d5e5fb597
}

void loop()
{
  HandleClient();
<<<<<<< HEAD
}
void HandleClient()
{
=======
  
}
void HandleClient()
{
  DynamicJsonBuffer jsonBuffer;
>>>>>>> 4b5e463ee9eb6c82b6551839dec9661d5e5fb597
  WiFiClient client = server.available();
  if (client)
  {
    currentTime = millis();
    previousTime = currentTime;
    Serial.println("New Client.");
    String currentLine = "";
    while (client.connected() && currentTime - previousTime <= timeoutTime)
    {
      currentTime = millis();
      if (client.available())
      {
        char c = client.read();
        Serial.write(c);
        header += c;
        if (c == '\n')
        { 
          if (header.indexOf("POST ") >= 0)
            {
              String line = client.readStringUntil('}');
              line += "}";
              JsonObject& root = jsonBuffer.parseObject(line);
<<<<<<< HEAD
              filename= root["filename"].as<String>();
              WriteToFile();
=======
              JsonToConfigStruct(root);
              //przeprowadz pomiar

>>>>>>> 4b5e463ee9eb6c82b6551839dec9661d5e5fb597
            }
          if (currentLine.length() == 0)
          {

            client.println("HTTP/1.1 200 OK");
            client.println("Content-type:application/json");
            client.println("Connection: close");
            client.println();
<<<<<<< HEAD
            WriteFileForClient(client);
=======
            
           
>>>>>>> 4b5e463ee9eb6c82b6551839dec9661d5e5fb597
          }
          else
          {
            currentLine = "";
          }
        } else if (c != '\r')
        {
          currentLine += c;
        }

      }

    } header = "";

    client.stop();
    Serial.println("Client disconnected.");
    Serial.println("");
   
  }
}
<<<<<<< HEAD

//void SD_file_download(String filename) {
//  File download = SPIFFS.open("/" + filename, "w");
//  if (download) {
//    server.sendHeader("Content-Type", "text/text");
//    server.sendHeader("Content-Disposition", "attachment; filename=" + filename);
//    server.sendHeader("Connection", "close");
//    server.streamFile(download, "application/octet-stream");
//    download.close();
//  } else ReportFileNotPresent("download");
//}
void WriteToFile()
{
    int i;
  
  File f = SPIFFS.open(filename, "w");
 if (!f) {
    Serial.println("file open failed");
  }
  else
  {
      Serial.println("Writing Data to File");
      Serial.println("start");
      //zaswiec diode
      delay(3000);
      startTime = millis();

      for (i = 0; i<1500;i++)
{
       bar.read();
       hammer.read();
       f.print(bar.a.z);
       f.print(',');
       f.print(hammer.a.z);
       f.print('\n');
 }
      currentTime = millis();
      timeS = (currentTime - startTime);
      f.print(timeS);
      f.close();  //Close file
      Serial.println("Data saved ");
  }
 }
 void WriteFileForClient(WiFiClient client)
 {
  int i;
   File f2 = SPIFFS.open(filename, "r");
   if (f2) 
  {
      for(i=0;i<f2.size();i++) //Read upto complete file size
      {
        client.print((char)f2.read());
      }
      f2.close(); 
  }
  }
 void ReadfromfileToSerial()
 {
  int i;
   File f2 = SPIFFS.open(filename, "r");
   if (!f2) {
    Serial.println("file open failed");
  }
  else
  {
      Serial.println("Reading Data from File:");
      //Data from file
      for(i=0;i<f2.size();i++) //Read upto complete file size
      {
        Serial.print((char)f2.read());
      }
      f2.close();  //Close file
      Serial.println("File Closed");
  }
  }
=======
void JsonToConfigStruct(JsonObject& root)
{

  configData.Mode = root["Mode"].as<String>();
  configData.ADR = root["AdaptiveDataRate"].as<String>();
  configData.DR = root["DataRate"].as<int>();
  configData.App_key = root["AppKey"].as<String>();
  configData.Dev_addr = root["DevAddr"].as<String>();
  configData.Join = root["Join"].as<String>();

}
void SD_file_download(String filename) {
  File download = SPIFFS.open("/" + filename, "w");
  if (download) {
    server.sendHeader("Content-Type", "text/text");
    server.sendHeader("Content-Disposition", "attachment; filename=" + filename);
    server.sendHeader("Connection", "close");
    server.streamFile(download, "application/octet-stream");
    download.close();
  } else ReportFileNotPresent("download");
}
>>>>>>> 4b5e463ee9eb6c82b6551839dec9661d5e5fb597
