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
              JsonToConfigStruct(root);
              //przeprowadz pomiar

            }
          if (currentLine.length() == 0)
          {

            client.println("HTTP/1.1 200 OK");
            client.println("Content-type:application/json");
            client.println("Connection: close");
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
