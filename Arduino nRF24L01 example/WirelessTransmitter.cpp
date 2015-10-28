#include "WirelessTransmitter.h"

WirelessTransmitter::WirelessTransmitter(uint64_t writePipe, uint64_t readPipe){
  Serial.println("WirelessTransmitter");  
  this->readPipe = readPipe;
    this->writePipe = writePipe;
    
    radio = new RF24(CE_PIN, CSN_PIN);
    wirelessInit();
}

WirelessTransmitter::~WirelessTransmitter(){
   delete(radio);
}

void WirelessTransmitter::wirelessInit(){
    radio->begin();
    radio->setAutoAck(true);
    radio->enableAckPayload();
    radio->powerUp();
    radio->setPALevel(RF24_PA_HIGH);
    radio->setDataRate(RF24_2MBPS);
    radio->setRetries(0,2); // Retry 2 times with 250us delay between retries.
    radio->openWritingPipe(writePipe);  
    radio->openReadingPipe(1,readPipe);
}

int WirelessTransmitter::getData(RequestsCodes type, int &data){
	radio->stopListening();
	int response;
	int request = type;
	bool success = radio->write( &request, sizeof(request) );
	radio->startListening();
	if(success){
		unsigned long started_waiting_at = micros();
		boolean timeout = false;
		while (!radio->available()){
			if (micros() - started_waiting_at > 200000 ){
				timeout = true;
				break;
			}
		}
		if(timeout) 
			return WIRELLES_RECEIVE_ERR;
		else{
			radio->read( &response, sizeof(response) );
			data = response;
			return WIRELLES_OK;
		}
	}else{
		return WIRELLES_TRANSMIT_ERR;
	}	
}

int WirelessTransmitter::getShadeTemperature(float &temp){
	int res = 0;
	int status = getData(SHADE_TEMPERATURE, res);
	temp = res/100.0;
	return status;
}

int WirelessTransmitter::getLightTemperature(float &temp){
	int res = 0;
	int status = getData(LIGHT_TEMPERATURE, res);
	temp = res/100.0;
	return status;	
}

int WirelessTransmitter::getIllumination(Illumination &illum){
	int res = 0;
	int status = getData(LIGHT, res);
	illum = Illumination(res);
	return status;		
}
