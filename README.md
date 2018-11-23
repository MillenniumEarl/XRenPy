# XRenPy
eXtended Ren'Py - visual constructor for Ren'Py (WIP)

- Currently only Windows
- This application allows developers to create simple Ren'Py game without any knowledge of Python and Ren'Py scripting.
- Created with Visual Studio 2015, programming language C#, .NET framework, Windows Presentation Foundation, LINQ and XAML.

Now it is possible to intuitively create and export to Ren'Py project:
- Kinetic novels
- Sound novels
- Simple ADV novels

Currently released application features:
- project structure block with labels containing frames and customizable size
- project resources block with 6 types of resources and customizable size
- creating and deletion of labels with editable names
- creating, duplicating and deletion of frames
- converting frames to menus and backwards
- menu options - can be viewed as editable structures with actions they have to do or as in-game clickable objects, doing the jump or call of the particular label or passing to the next frame
- importing images, audio and movies to the novel (no duplicate check)
- adding images, audio and movies to the frame by checking them (backgrounds and movies - one per frame, only background)
- strict system of directories inside the project - now the content is where it has to be
- content collector - copying of the particular resource to the project folder when necessary
- media preview - if cursor is above any resource, the media preview window shows up, on leave it closes, on click freezes in opened state and, if the resource is present in currently selected frame, shows the parameters of this resource below it. Contains image viewer, music player with cover art and controls and movie player with the same controls
- images alignment and animation controls
- audio fades controls (still no value check)
- view block - visual representation of the prototype of the future game, contains six buttons for converting menu to frame and backwards, adding empty frame, inserting empty frame, duplicating current frame and two control buttons to move to next or previous frame
- view block size customizing

Features to add:

- extend space of any resource for cursor to enter
- editable frame name (maybe containing the text it shows - very useful)
- duplicate checking of resources
- different image and movies sizes ans placement
- multiple movies at once (not actually necessary, but who knows)
- connection to the options.rpy and gui.rpy scripts
- draggable tabs in structure tab control
- draggable visual elements in the view block with effect
- audio fades value check (we don't want to type in the word and get an error, do we?)
- project cleaning - removal of unnecessary resources
- new project manipulation logic:
- duplication of frames is primary action, inserting empty frames is secondary;
- merging of "add" and "insert" to inserting empty frame; all resources have three-state checkboxes;
- on selection of resource the frames it is shown in are also highlighted;
- on selection of frame the resources that are shown/played in it are ticked and highlighted, the resources that are shown somewhere in previous frames are also ticked, but when developer wants to remove any used resource before frame will appear, he ticks it as indeterminate state, which causes the removal of resource tick in all frames below;
- log system
- updater service

Things to solve:
- parasite click on the border of any resource and menu options which can cause an unwanted but not causing problems action
- label name changing has to cause the menu options label's name to change name too
- unexpected grid behaviour while changing the size of structure and resource blocks
- strange program fail to start while being transferred to another PC
