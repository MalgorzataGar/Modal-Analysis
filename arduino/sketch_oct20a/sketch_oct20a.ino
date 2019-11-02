
#include <Wire.h>
#include <LSM303.h>

LSM303 bar;
LSM303 hammer;

int barData[200];
int hammerData[200];


unsigned long startTime;
unsigned long currentTime;
unsigned long timeS;


char report[80];
void setup()
{
  Serial.begin(9600);
  Wire.begin();
  bar.init(LSM303::device_auto, LSM303::sa0_low); //SDO do GND
  hammer.init(LSM303::device_auto, LSM303::sa0_high);
  bar.enableDefault();
  hammer.enableDefault();
  startTime = millis();
}

void loop()
{
 
 getData();
}
void getData()
{
  int i;
  Serial.println("start");
  delay(3000);
  startTime = millis();
  for (i = 0; i<150;i++)
  {
    
    bar.read();
    hammer.read();
    barData[i] = bar.a.z;
    hammerData[i]= hammer.a.z;//a nie os -x
   }
    currentTime = millis();
    timeS = (currentTime - startTime);
    for (i = 0; i<150;i++)
  {
     snprintf(report, sizeof(report), "%d,%d",//to moze byc problemem
    barData[i],hammerData[i]);
    Serial.println(report);
    delay(100);
   }
   Serial.println(timeS);
    delay(5000);
 }
