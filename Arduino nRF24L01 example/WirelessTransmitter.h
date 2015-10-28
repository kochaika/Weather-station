
#ifndef WIRELESSTRANSMITTER_H
#define WIRELESSTRANSMITTER_H

#include <RF24.h>

#include "RequestsCodes.h"
#include "Illumination.h"

#define CE_PIN 7
#define CSN_PIN 8

#define WIRELLES_OK 0
#define WIRELLES_TRANSMIT_ERR -1
#define WIRELLES_RECEIVE_ERR -2


class WirelessTransmitter{  
public:

    WirelessTransmitter(uint64_t writePipe, uint64_t readPipe);
    
    ~WirelessTransmitter();
   
    int getShadeTemperature(float &temp);
    int getLightTemperature(float &temp);
    int getIllumination(Illumination &illum);
    
private:
    //Initialization of the transmitter
    void wirelessInit();

    int getData(RequestsCodes type, int &data);
	    
    //Receiving pipe
    uint64_t readPipe;
    //Transmission pipe
    uint64_t writePipe;
    //Transmitter
    RF24 *radio;
};

#endif
