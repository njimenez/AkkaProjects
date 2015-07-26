
# WinTail 

*NIX systems have the `tail` command built-in to monitor changes to a file (such as tailing log files), whereas Windows does not. We will recreate `tail` for Windows, and use the process to learn the fundamentals.

This is a WPF application that will show you a list of files to select to monitor the changes to those files.
It will open a new window per file, so that one can monitor multiple files.

## Main Window
![wintail-mainwindow-01](images/mainwindow-01.png)

## Main Window after clicking the crawl button

![wintail-mainwindow-02](images/mainwindow-02.png)

The Main window consists of two textboxes, a button, and a List box. The top textbox will have the directory to search, and the second textbox will will have the search pattern. The `crawl` Button will start things off and the results will be displayed in the List box.   

From the Listbox you can double click on an item to start monitoring it. This will open a new window and display the last 1500 lines of the file.

## Message Flow Diagram

![message-flow-diagram](Images/message-diagram.png)


# Actors Involved

## WinTailSupervisor
## FileValidatorActor
## FileEnumeratorActor
## FileReaderActor
## TailCoordinatorActor
## TailActor

