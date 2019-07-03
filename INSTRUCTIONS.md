# XRen'Py: How to make a game

## Getting started
XRen'Py is a tool for fast prototyping of visual novels and creating simple visual novels using Ren'Py. It covers the basic functionality of Ren'Py visual novel engine.
Now if you want to start creating games with XRen'Py you better have:
- Latest Ren'Py 7, you can get it here: https://www.renpy.org/latest.html
This is not necessary to run the program, but obviously it is needed to create a game.
- Windows Media Player installed to play music or videos in XRen'Py.
If you are not sure it is present in your Windows, you can go to the Control Panel to install and configure system components.
- .NET Framework 4.5
- free folder for new projects.

## Important things you have to know
Every project in Ren'Py 7 consists of folder game, which contains four files: script.rpy, screens.rpy, options.rpy and gui.rpy, and image folder. 
**script.rpy** is the most important file to you - it is the script of your game.
**screens.rpy** contains info about all of the screens that might be necessary for game, for example, main menu screen.
**options.rpy** is a file that contains options of the game, such as name, audio settings, transitions between screens, etc.
**gui.rpy** was added in Ren'Py 7 and it collected all the options that game developer needs to customize game graphics.
Image folder is the most basic folder in Ren'Py. In Ren'Py it contains image resources of the game.

## Opening the XRen'Py
In downloaded binaries, select necessary version based on your PC OS version (x86 or x64) and run XRen'Py.exe. You can run x86 files if you are not sure.

## Graphic user interface
Program is divided on:
- project block at the left side of window which consists of main menu and options block at the top left corner, structure block and resources block below it,
- view block at the right side of window.
**Project resource block** is upper part of your script.rpy called init block. It contains all the content you need to create a game.
It includes background images, character images, music, sound or voice files, and also movies as cutscenes and side images.
**Project structure block** is lower part of your script.rpy. It contains the collection of labels that represent the branches of script.
Labels are the most basic things in script - they contain frames that represent a moment of game. 
Frame is based on text that tells any character, including author or narrator. Frame also can contain any resources allowed.
**View block** is the main part of XRen'Py. It gives access to the character manager, GUI settings, audio mixer, frame creation and next/previous frame.
View block also shows all the contents of current frame, including text and character.
Characters are the part of init block in script.rpy, but they are placed in the view block because they require separate manager and are used by current frame.
GUI is connected to gui.rpy. It is placed in view block for obvious reasons.
Audio mixer plays all audio connected to the current frame.
Add frame button inserts one frame after the current and shows it.

## Using the XRen'Py

### Project folders
Immediately after opening, the XRen'Py shows the project that is similar to Ren'Py's default project, except script. 
By default, new project of XRen'Py is created in the XRen'Py folder called "temp". 
If you need another folder to contain new game or to save current project to a new folder, open the project main menu and select Save As... to save the project to a new folder.
**Note that XRen'Py doesn't create new folder (yet) by itself so you will have to create it by yourself before saving.**
XRen'Py makes subfolders for all types of content and connects them to init block by default.

## Writing simple game

### Adding new frame
To add new frame click right mouse button on current label or "add frame" button in the view block.

### Editing frame text
Select frame that you want to edit and write any text in the text box in view block. Simple, isn't it?

### Using characters
Ren'Py has predefined characters such as none, centered, extend and nvl. 
none means that text is spoken by narrator, centered makes all the text be in the center of screen, extend makes he text shown phrase by phrase, nvl makes the wall of text be shown on the whole screen.
In XRen'Py these characters are not editable but usable.
**Note that selection of these characters doesn't affect (yet) view block text box positioning and character label visibility.**
To add your own character simply type his name, select the style his name and text will be shown with and colors, then click Add.
**You can also add an icon of this character - it affects the game as the result but doesn't affect the view block.**
Remember that default colors are defined in GUI and used when selecting Transparent. 
To use character in the current frame click on any character in the list and click Select.
While adding a new frame the character from the currently used frame stays in the new frame so you don't have to select the same character few times.

### Resource management
To add new resource click right mouse button on current tab in resource block and select Add.
If you want to change the resource contents, for example image, without any losses of it's connection to frames in project, select necessary frame and click right mouse button on it, select Reload.
If you want to remove it - click right mouse button on resource, select Delete.
**Note that XRen'Py has a content collector which means any resource you selected to load into the project will be copied to the project's subfolder depending on kind of content loading so you don't have to do it by yourself.**
While resource is entered, it is shown in media viewer, or actually previewer - so you don't have to select anything to see what it is. 
Selection of anything will make it appear in the media viewer and freezes it in opened state.
Checking the resource or selecting a checked item also freezes it and shows the panel of properties where you can select how do you want this resource to be displayed/played in game.

### Connecting resources to frames
Every resource has checkbox with three states. 
First state - checked. If you checked resource then it is shown in current frame (cutscenes only) and all the frames after (images and music).
Second state - indeterminate. It means resource was checked somewhere in previous frames and still continues to be shown.
Third state - unchecked. It means that resource haven't been selected or stopped to be visible/audible in current frame or somewhere previously.
Checked state of all resources depends on frame you selected. By checking unchecked resources you make them appear in this frame, by unchecking - disappear.

### Making branches
To make a new script branch, click on the plus button in the project structure block. Created label will contain one empty frame.
Double click on the name of label allows you to change it, the tick on the left allows you to apply the name you wrote.
**Note that start label can't be renamed or deleted.**

### Making menus
Menu is the one and only way to access the branch you have created. 
There are two ways of creating menus - from existing frame by clicking the right mouse button on it and selecting Frame->Menu or by clicking on free space of list and selecting Add menu.
The panel at the center of view block will consist of menu options.
Clicking on pencil button will show you the inner look of menu option - the choice, the action after selecting and the label you want to go to.
Ren'Py pre-defines three actions - jump, call or pass. 
*jump* means to go to another label and stay there.
*call* means to go to another label, but on it's end return to this label.
*pass* means to go to the next frame of current label.
When pass is selected, you can't choose the label you want to go to - you stay in this label anyway.
Clicking on plus button will add new option, clicking on delete button will remove it.
Clicking on pencil button again will hide the inner choice actions and make the choice clickable. 
When the menu option in this state, clicking on choice will apply the action and select next frame (pass) or the label you wanted it to go to.
**Note that call action will not send you to previous label after it's done but will affect the final game.**

### Saving project
Clicking on Save button will save current project in selected folder.
Clicking on Save As button will open the window to save the project there and saves the game there.

### Using Ren'Py
To create and build the game you have to use Ren'Py. Launcher of Ren'Py gives you ability to compile the project you're creating with XRen'Py.
**Note that XRen'Py is not connected in any way to Ren'Py and to start/build the game you need the game folder to be in the projects folder that was set in Ren'Py.**
Here is the guide about it:
https://www.renpy.org/doc/html/quickstart.html#releasing-your-game

That's all you need to know to start using XRen'Py!

