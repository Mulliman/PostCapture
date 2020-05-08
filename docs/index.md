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
