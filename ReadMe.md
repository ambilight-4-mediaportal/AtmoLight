AtmoLight MediaPortal Plugin
============================

AtmoLight is a plugin for MediaPortal1 and MediaPortal2 to add support for ambilight systems.


## Features

 * __Control__ your Amibilight from within MediaPortal
 * Support for different targets: __[AtmoWin](https://github.com/ambilight-4-mediaportal/AtmoWin)__, __[Hue](https://github.com/ambilight-4-mediaportal/AtmoHue)__ and __[Hyperion](https://github.com/tvdzwan/hyperion)__
 * Ability to use __more than one__ target __at the same time__
 * __Internal capture mode__ to reduce cpu load
 * Change effects __automatically__, depending on the media type that is playing
 * Use your __remote control__ to toggle leds, change the profile of your target software or open up the context menu (only MediaPortal1)
 * Setting to lower the capture frequency for lower end pcs
 * __Delay option__, that lets you add a delay before the current picture gets displayed on the leds
 * __Blackbar detection__ and __removal__ (only MediaPortal1)
 * __GIF reader__ effect, where users can define a gif file which then gets played back on the leds
 * __VU meter effect__, watch a nice light show while listening to your music
 * Define a timeframe in which your ambilight should be deactivated


## Useful Links

 * [Google code project page](https://code.google.com/p/ambilight-4-mediaportal/)
 * [Download Archive](https://ambilight-4-mediaportal.googlecode.com/git/MPEI%20Release/Atmolight/)
 * [Patched MediaPortal.exe Archive](https://ambilight-4-mediaportal.googlecode.com/git/MediaPortal/MediaPortal.exe/)
 * [IRC Webchat (#ambilight4mediaportal @ freenode)](http://webchat.freenode.net/)
 * [AtmoLight for MediaPortal 1 Forum thread](http://forum.team-mediaportal.com/threads/atmolight-1-13-0-0-2014-06-17.125633/)
 * [AtmoLight for MediaPortal 2 Forum thread](http://forum.team-mediaportal.com/threads/atmolight-2-0-0-0-beta-1-for-mediaportal2-development-discussion-test-version-thread.125674/)
 * [AtmoWin Forum thread](http://forum.team-mediaportal.com/threads/atmowin-release-thread-no-bug-support.125361/)
 * [AtmoHue Forum thread](http://forum.team-mediaportal.com/threads/atmohue-beta-philips-hue-support-for-atmolight-atmowin.128252/)
 * [AtmoWakeHelper Forum thread](http://forum.team-mediaportal.com/threads/solution-for-auto-com-reconnect-on-sleep-resume-beta4.126160/)
 
 
## Frequently Asked Questions

__Question:__ What is AtmoLight and AtmoWin, which do I need and why?
__Answer:__ AtmoWin is a standalone software that analyses the content on you screen and then sends the colors to your Ambilight controller (Arduino, SEDU and so on). AtmoLight is a MediaPortal plugin that allows to send the screen content directly to your target software (e.g. AtmoWin), making this way more efficient and cpu friendly. AtmoLight also acts as a remote for your target software (e.g. AtmoWin), allowing the user to enable or disable the leds, aswell as change effects and lots more. It is highly recommended that you use the AtmoLight plugin, mainly to ensure smooth video playback.


__Question:__ My video playback stutters/I have a lot of dropped frames. How can i fix this?
__Answer:__ Make sure that AtmoLight uses the "MediaPortal Liveview Mode". This way AtmoLight handles the capturing of the screen and not your target software. As AtmoLight can work directly with MediaPortal rendering assets this is way faster. Open the MediaPortal Configuration, navigate to Plugins and open the AtmoLight configuration. For Video/TV/Recordings choose "MediaPortal Liveview Mode". Click Save and exit MediaPortal Configuration with OK.


__Question:__ AtmoLight cant connect to AtmoWin, what can i do?
__Answer:__ AtmoWin needs to register its COM interface first. To achieve this, open the command prompt (Win+R and type cmd), navigate to the AtmoWin directory (C:\ProgramData\Team MediaPortal\MediaPortal\Atmowin) and then execute "AtmoWinA.exe /register".
If you have connection issues after resume, you should take a look at this thread:
http://forum.team-mediaportal.com/t...o-com-reconnect-on-sleep-resume-beta4.126160/


__Question:__ I can't open the context menu/toggle leds/switch profile with the color buttons. What is wrong (MediaPortal 1)?
__Answer:__ You will need to add some new actions to the remote buttons. Open MediaPortal Configuration and navigate to Remotes. In the tab for you remote press on Mapping (for some remotes press Learn and then Mapping). Navigate to "Teletext specific buttons", here you can see the 4 color buttons. Open the button you want to use (e.g. red). Under "No Condition" you will have to add the action "Remote Red Button". This way everytime you press red, no matter where you are in MediaPortal, the "Remote Red Button" action gets triggered and AtmoLight gets informed. Repeat this for all the buttons you want to use.
![](http://forum.team-mediaportal.com/attachments/upload_2014-5-25_16-8-8-png.151019/)


__Question:__ I choose "MediaPortal Liveview Mode" for Music, Radio or GUI/Menu, but its not working. Why (MediaPortal 1)?
__Answer:__ To use the MediaPortal Liveview Mode outside of video playback you will need a patched MediaPortal.exe. You can find the exe here: https://ambilight-4-mediaportal.googlecode.com/git/MediaPortal/MediaPortal.exe/
If you use a version that has no patch yet, please let me know and i will generate one for you.


__Question:__ What does the frequency mean for the delay feature?
__Answer:__ During implementation of the delay feature we discovered that the delay depends on the refresh rate of your monitor/tv. For example, on the same system a video played with 50hz needs more delay than a video played with 24hz. AtmoLight can calculate the delay for every refreshrate, if you define one delay at one refresh rate. To set this up, simply start a video, figure out the needed delay and then note the refresh rate the video was playing at.