---
layout: default
---

# How it works

PostCapture consists of two fundamental parts, the `studio` and the `processor`.

In the `studio` you can create a set of `processes` which will perform a series of `operations` on an image.

Then you can pass JPEG images into the `processor` and it will apply all the `operations` set up for the matching `process`.

The matching `process` will be determined by the metadata on the image matcing the criteria set up on each `process`.

## Setting up your first process

<div class="youtube-container">
<iframe src="https://www.youtube.com/embed/RIO7unaJuGY" frameborder="0" allow="accelerometer; autoplay; encrypted-media; gyroscope; picture-in-picture" allowfullscreen></iframe>
</div>

1. Using the 'Create new process section' input a name for your process and press 'Create'.
1. See how the new process appears in your list of processes and is selected by default. You can now configure your process.
1. Enter values into the category and value text boxes. This is the metadata that will be used to determine when this process will be run. 
1. Processes are ordered by priority. Only the first matching process is run for each image, so priority is important to ensure that the correct process is getting run.
The lower the number, the earlier it will be tested for a match with the current image. Higher numbers can be used to denote that a process is a fallback/default like in this example.
1. You can now add steps (operations) to your process from the panel at the bottom right of your screen by clicking on one of the options.
1. Once an operation has been added you can set your preferences and the preview will automatically be updated to show an example of how it will work. 
1. Be sure to save your process using the button at the top right before you close the program or attempt to apply the process to an image.

## Setting up a more advanced process

<div class="youtube-container">
<iframe src="https://www.youtube.com/embed/ThFwQhkOg1E" frameborder="0" allow="accelerometer; autoplay; encrypted-media; gyroscope; picture-in-picture" allowfullscreen></iframe>
</div>

1. With a process already selected using the 'Create new process section' input a name for your process and select 'Copy settings from selected process'.
This will automatically populate the operations of the new process with those from the selected one.
1. Selecting 'Populate rule from example image' will allow you to choose an image that contains the metadata you want to match with. 
The example here shows how the color tags in Capture One can be used to choose when certain processes run. 
Once selected the drop down list will contain all the metadata categories and values, and if you select one this will automatically be set as the match criteria of the new process.
1. Press 'Create copy' to make the new process.
1. You can now change any existing operations on this new process without affecting the process from which it was copied.
1. You can also add watermarks from the "Add extra steps to process" section. Choose your image and position as needed.
1. Operations can be moved up and down using the arrows at the top of each one. The order in which operations are applied can affect how the output is rendered.
1. You can add multiple instances of each operation to achieve more complex outputs.

## Manually running the process from file explorer

<div class="youtube-container">
<iframe src="https://www.youtube.com/embed/wQUsl2ntKBw" frameborder="0" allow="accelerometer; autoplay; encrypted-media; gyroscope; picture-in-picture" allowfullscreen></iframe>
</div>

1. To run the image processor included in PostCapture all you need to do is drag the selected image on top of the file "PostCapture.Process.exe".
1. The two images shown here have different metadata and because of the two processes setup above you can see that the output is different.
1. In the current beta version you can only do one image at a time, but you'll be able to do multiple soon.

<hr style="margin-top:3rem" />

# Capture One

## Installation

PostCapture was designed to work with Capture One, and installing it is very simple.

<div class="youtube-container">
<iframe src="https://www.youtube.com/embed/SasSxpPfL8U" frameborder="0" allow="accelerometer; autoplay; encrypted-media; gyroscope; picture-in-picture" allowfullscreen></iframe>
</div>

1. <a href="https://github.com/Mulliman/PostCapture/blob/master/Docs/assets/releases/PostCapture.CaptureOne.coplugin?raw=true">Download the coplugin here</a>
1. Open Capture One and open preferences from the edit menu.
1. Select the plugins tabs press the plus button bottom left.
1. Choose the downloaded coplugin file and press open.
1. PostCapture is now installed.
1. Press 'Open Configurator' to ensure that the studio opens successfully. 

## Opening PostCapture Studio from Capture One

As seen above, you can open the studio from the plugins section in the user preferences, but you can also use Capture One's open with functionality on JPEGs or edit with if you export as a JPEG.
The benefit of this approach is that the 'Populate rule from example image' section will be pre-filled with data from the selected image.

<div class="youtube-container">
<iframe src="https://www.youtube.com/embed/RqI4tz8QYTo" frameborder="0" allow="accelerometer; autoplay; encrypted-media; gyroscope; picture-in-picture" allowfullscreen></iframe>
</div>

1. Right click on a JPEG image and hover over Open With and choose 'Set up PostCapture process'.
1. If your image is not a JPEG, you need to use Capture One's 'edit with' menu instead and export as a JPEG. 
This created image will only be temporarily stored until the next time you use any of the 'edit with' options shipped with PostCapture. 
In the metadata options of this export dialog, make sure you have 'Rating and Color Tag' checked if you want to use those tags in your processes.

## Running the processor on images from Capture One

There are two ways to directly run the proessor on an image in Capture one, "Open With" and "Edit With".
If you have a JPEG and want to overwrite the original image with the image processed with PostCapture then choose "Open With".
It is recommended to use "Edit With" though. This will save the processed image to a new file alongside the original so you can always roll back.
You can also use "Edit With" to run the process on RAW images or Tiffs which currently aren't supported by PostCapture. Just make sure you choose JPEG as the file type when exporting.

<div class="youtube-container">
<iframe src="https://www.youtube.com/embed/AW3xLKlumw8" frameborder="0" allow="accelerometer; autoplay; encrypted-media; gyroscope; picture-in-picture" allowfullscreen></iframe>
</div>

1. To create a new image with the changes made by PostCapture, right click your image, go to the 'Edit With' menu and click 'Apply PostCapture process'.
1. Choose JPEG as the format in the 'Basic' tab.
1. Ensure that any Metadata that your processes use are included by checking the relevant boxes on the 'Metadata' tab.
1. Press the button and the process will run automatically.
1. If you want to update the existing image (not recommended) right click your image, go to the 'Open With' menu and click 'Apply PostCapture process'. The process will run automatically.

## Adding the processor to process recipes in Capture One

The recommended approach for using PostCapture is to use it within process recipes in Capture One. This is very easy to do.

<div class="youtube-container">
<iframe src="https://www.youtube.com/embed/8giv_z026D0" frameborder="0" allow="accelerometer; autoplay; encrypted-media; gyroscope; picture-in-picture" allowfullscreen></iframe>
</div>

1. Select the 'Output' tab within Capture One.
1. Choose the process recipe that you want to add the PostCapture processor to.
1. In the 'Process Recipe' tool, on the 'basic' tab find the 'Open With' drop down list. Select 'Apply PostCature process'.
1. Ensure that any Metadata that your processes use are included by checking the relevant boxes on the 'Metadata' tab.
1. With the images you want to process selected as well as the recipe set up click the process button.