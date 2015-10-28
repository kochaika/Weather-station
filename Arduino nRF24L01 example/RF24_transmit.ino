#include <SPI.h>
#include <nRF24L01.h>
#include <RF24.h>
#include <RF24_config.h>

#include "WirelessTransmitter.h"


const uint64_t pipe = 0xF0F0F0F0E1LL;
const uint64_t pipe1 = 0xF0F0F0F0D2LL;

const int buttonPin1 = 2; 
const int buttonPin2 = 3; 
const int buttonPin3 = 4; 
int buttonState1 = 0;
int buttonState2 = 0;
int buttonState3 = 0;

char lightLevels[][12]={
    "Dark", 
    "Very cloudy",
    "Cloudy", 
    "Clear", 
    "Very sunny"
    };

WirelessTransmitter *radio;

Illumination lightnes;
float temp;
int stat;

void setup() {
  Serial.begin(9600);
  radio = new WirelessTransmitter(pipe, pipe1);
  Serial.println("Base started!");
}



void loop() {
    
    buttonState1 = digitalRead(buttonPin1);
    buttonState2 = digitalRead(buttonPin2);
    buttonState3 = digitalRead(buttonPin3);
    if( buttonState1 == HIGH || buttonState2 == HIGH || buttonState3 == HIGH)
    {
      if(buttonState1 == HIGH){
         stat = radio->getShadeTemperature(temp);
         switch(stat){
         case WIRELLES_OK:
              Serial.print("Shade temperature = ");
              Serial.println(temp);
              break;
         case WIRELLES_TRANSMIT_ERR:
              Serial.println("Transmit error");         
              break;
         case WIRELLES_RECEIVE_ERR:         
              Serial.println("Receive timed out");         
              break;  
         }       
      }
      if(buttonState2 == HIGH){
         stat = radio->getLightTemperature(temp);
         switch(stat){
         case WIRELLES_OK:
              Serial.print("Light temperature = ");
              Serial.println(temp);
              break;
         case WIRELLES_TRANSMIT_ERR:
              Serial.println("Transmit error");         
              break;
         case WIRELLES_RECEIVE_ERR:         
              Serial.println("Receive timed out");         
              break;  
         }      
      }
      if(buttonState3 == HIGH){
         stat = radio->getIllumination(lightnes);
         switch(stat){
         case WIRELLES_OK:
              Serial.print("Illumination level = ");
              Serial.println(lightLevels[lightnes]);
              break;
         case WIRELLES_TRANSMIT_ERR:
              Serial.println("Transmit error");         
              break;
         case WIRELLES_RECEIVE_ERR:         
              Serial.println("Receive timed out");         
              break;  
         }       
      }
      
      delay(100);
    }

}
