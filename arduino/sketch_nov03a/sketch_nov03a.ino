#include <Wire.h>
#include <LSM303.h>
#include<ESP8266WiFi.h>
#include <FS.h>

LSM303 bar;
LSM303 hammer;
unsigned long startTime;
unsigned long currentTime;
unsigned long timeS;
const char* filename = "/pomiar052.txt";
void setup() {
  Serial.begin(115200);
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

void loop() {
  int i;
  
  File f = SPIFFS.open(filename, "w");
 if (!f) {
    Serial.println("file open failed");
  }
  else
  {
      Serial.println("Writing Data to File");
      Serial.println("start");
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
  
    delay(20000);
}


 


 
  
