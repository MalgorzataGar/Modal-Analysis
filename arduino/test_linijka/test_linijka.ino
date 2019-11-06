
#include <WiFiClient.h>

#include <Wire.h>
#include <LSM303.h>
#include <SD.h>
#include <SPI.h>

LSM303 bar;
LSM303 hammer;
//File file;

const int16_t I2C_SLAVE = 0x08;
int CS_PIN = 10;
unsigned long startTime;
unsigned long currentTime;
unsigned long timeS;


void setup()
{
  Serial.begin(115200);
  Wire.begin(2,1);
 
  //bar.init(LSM303::device_auto, LSM303::sa0_high);
  //bar.enableDefault();

}

void loop()
{
 
 getData();
}
void getData()
{
  
//  short int barData[200];
//  int i;
//  Serial.println("start");
//  delay(3000);
//  startTime = millis();
//  //initializeSD();
// // createFile("p1.txt");
//  for (i = 0; i<200;i++)
//  {
//    
//    bar.read();
//    barData[i] = bar.a.z;
//  // file.println(bar.a.z);
//  // file.println("\n");
//    
//   
//   }
//    currentTime = millis();
//    timeS = (currentTime - startTime);
    //file.println(timeS);
   //file.println("\n");
//    closeFile();
//    openFile("p1.txt");
  for (int i = 0; i<200;i++)
  {
      Serial.println("Hello");
     //Serial.println(barData[i]);
   }
  // Serial.println(timeS);
   //closeFile();
   // delay(5000);
 }
//void initializeSD()
//{
//  Serial.println("Initializing SD card...");
//  pinMode(CS_PIN, OUTPUT);
//
//  if (SD.begin())
//  {
//    Serial.println("SD card is ready to use.");
//  } else
//  {
//    Serial.println("SD card initialization failed");
//    return;
//  }
//}
// int createFile(char filename[])
//{
//  file = SD.open(filename, FILE_WRITE);
//
//  if (file)
//  {
//    Serial.println("File created successfully.");
//    return 1;
//  } else
//  {
//    Serial.println("Error while creating file.");
//    return 0;
//  }
//}
//
//int writeToFile(int text)
//{
//  if (file)
//  {
//    file.println(text);
//   
//    return 1;
//  } else
//  {
//    Serial.println("Couldn't write to file");
//    return 0;
//  }
//}
//
//void closeFile()
//{
//  if (file)
//  {
//    file.close();
//    Serial.println("File closed");
//  }
//}
//
//int openFile(char filename[])
//{
//  file = SD.open(filename);
//  if (file)
//  {
//    Serial.println("File opened with success!");
//    return 1;
//  } else
//  {
//    Serial.println("Error opening file...");
//    return 0;
//  }
//}
//
//String readLine()
//{
//  String received = "";
//  char ch;
//  while (file.available())
//  {
//    ch = file.read();
//    if (ch == '\n')
//    {
//      return String(received);
//    }
//    else
//    {
//      received += ch;
//    }
//  }
//  return "";
//}
