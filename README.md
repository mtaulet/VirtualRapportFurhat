# Virtual Rapport with Furhat

Research has demonstrated that creating rapport during social interactions has positive effects in engagement and connection. In situations when a human interacts with a social robot, one way of increasing rapport is through the synthesis of nonverbal backchannels from the robot at appropriate times. In this project, we build a prototype system that equips social robots with rapport capabilities. We use [Furhat](https://furhatrobotics.com/), a social robot with human-like expressions and conversational capabilities. We connect Furhat with the multimodal learning platform [OpenSense](https://github.com/ihp-lab/OpenSense/tree/master) through the creation of two new components:

1. **Virtual Rapport System** is a component that takes in the inputs from OpenSense (Head Gesture, Acoustic Features and Voice Activity Detection) and maps them to Furhat Gestures following a set of rules by [Gratch et al](https://ict.usc.edu/pubs/Virtual%20Rapport.pdf) This component is meant to be a proof of concept to establish the connection between Furhat and Opensense and the behavior can be expanded and built upon in the future.

2. **Furhat Controller** is a component that serves as the interface to Furhat. It takes in the gesture commands produced by the Virtual Rapport System and issues calls to the Furhat Web API for the gestures to be synthesized by the robot. This controller is decoupled as an independent component so it can be used with other OpenSense components that interact with Furhat.

These are the rules implemented by the Virtual Rapport System:

1. **Lowering of pitch -> head Nod**

Used [OpenSMILE ](https://audeering.github.io/opensmile/get-started.html#default-feature-sets)component to extract feature F0\_sma\_linregc1 (feature 462). The config file used is [emobase\_live4.config](https://github.com/ihp-lab/OpenSense/blob/master/Documents/WPF%20Application%20Samples/5%20-%20openSMILE%20for%20signal%20processing/openSMILE/sample2/opensmile_emobase_live4.conf)  This is the slope of F0. If a pitch within human speech is detected and the slope is lower than a threshold it means the speaker has lowered their pitch, and the robot nods.

1. **Raised loudness -> head Nod**

Similarly, we used the OpenSMILE component to extract feature pcm\_loudness\_sma\_linregc1 (feature 25) to extract the slope of loudness. If it’s higher than a threshold then the robot nods.

2. **Speech disfluency -> Gaze shift**

We use the Voice Activity Detector component with a pause length of 2 seconds, meaning that if the speaker is quiet for more than 3 seconds the robot will shift the gaze to signal to the speaker that they can take their time. 

3. **Head nod/head shake -> mimic** 

We use the Head Gesture Detector with OpenFace to detect and mimic Head Nods and Shakes. 

There is a minimum 5 second spacing in between gestures so that the backchannels from Furhat do not become overwhelming or repetitive.

## Steps to run

- Power on Furhat robot and connect it to the network. Get its IP address following the [instructions](https://docs.furhat.io/internet/#step-3-connect-furhat-to-your-wifi).

- Go to the [Furhat Studio](https://docs.furhat.io/robot/#accessing-the-robot-in-furhat-studio) and activate the Web API button to run the API server.

- Make sure you have a camera available to OpenSense (computer camera or external webcam).

- Open OpenSense environment and check that the IP address of your Furhat robot matches that variable `furhatUri` in FurhatController.cs.

- Run OpenSense as you would normally. The custom pipeline sample is recommended. Make sure your OpenFace parameters are correctly set according to your camera parameters.

## Demo
Access a demonstration video here: https://drive.google.com/drive/folders/1eATIo7hI24wXU18HQSh_lZaBpcScT31d?usp=sharing

### To improve/ Future work

- Allow Furhat’s URI to be configured as a parameter in the visual UI instead in the source code.
- Have a hierarchy of what backchannel should be displayed when multiple gestures are detected on the speaker.
- Implement the detection of speaker gaze shift and mimic.
