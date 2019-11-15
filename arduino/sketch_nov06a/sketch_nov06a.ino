#include <ESP8266WiFi.h>
#include <ArduinoJson.h>
#include <WiFiClient.h>
#include <ESP8266WebServer.h>
#include <Wire.h>
#include <LSM303.h>
#include <FS.h>

LSM303 bar;
LSM303 hammer;

char filename[20];

const char *ssid = "WLAN1-002093";
const char *password = "7277AF0C32C26AC";
WiFiServer server(80);
const int output = D0;
String header;
unsigned long startTime;
unsigned long timeS;
unsigned long currentTime=millis();
unsigned long previousTime = 0;
const long timeoutTime = 5000;

void setup() {
  Serial.begin(115200);

  Serial.print("Connecting to ");
  Serial.println(ssid);
  WiFi.begin(ssid,password);
   pinMode(output, OUTPUT);
  digitalWrite(output, LOW);
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

}

void loop()
{
  HandleClient();

}

void HandleClient()
{
  DynamicJsonBuffer jsonBuffer;
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
      Serial.println(client.available());
      if (client.available())
      {
        char c = client.read();
        Serial.write(c);
        header += c;
        if (c == '\n')
        { 
          
          if (currentLine.length() == 0)
          {
            if (header.indexOf("POST ") >= 0)
            {
              String line = client.readStringUntil('}');
              line += "}";
              
              JsonObject& root = jsonBuffer.parseObject(line);
              String filenameBuffer= root["filename"].as<String>();
              filenameBuffer.toCharArray(filename,sizeof(filename));
              Serial.println(filename);
              WriteToFile();
            }
            
            client.println("HTTP/1.1 200 OK");
            client.println("Content-type:text/plain");
            client.println("");
            
            WriteFileForClient(client);  
            //client.println("Connection: close");
            client.println();

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
      digitalWrite(output, HIGH);
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
      digitalWrite(output, LOW);
      f.print(timeS);
      f.close();  //Close file
      Serial.println("Data saved ");
  }
 }
 void WriteFileForClient(WiFiClient client)
 {
  int i;
  char a;
  Serial.println("Writing data for client");
   File f2 = SPIFFS.open(filename, "r");
   if (f2) 
  {
      for(i=0;i<f2.size();i++) //Read upto complete file size
      {
        a=(char)f2.read();
        client.print(a);
        
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
