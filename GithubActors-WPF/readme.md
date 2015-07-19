
# Akka.NET Bootcamp - Unit 3: Advanced Akka.NET 
# using WPF


Here we are going to show, one way to integrate Akka.Net and WPF with MVVM using Reactive UI.

## Concepts we'll explore
1. How to integrate Reactive UI with WPF
2. How to use ViewModels to interact with the actors


## Preparation
### Step 1 - Setup GitHubAuth.XAML

Created a `ReactiveObject` ViewModel to bind with the form.
 
The ViewModel called `GithubAuthViewModel` takes the form as a parameter so that we can hide it after being authorized.

Setup some properties to bind the form with:

1. `OAuthToken` property that binds to the textbox where we input the Github Token.
2. `Status` property that binds to the StatusBar on the form.    
3. `StatusLabelForeColor` property that binds to the color of the status message.
4. `GetHelp` command that binds to the Hyperlink to get Github help for the Token
5. `Authenticate` command to kick off the authentication

In the constructor of the ViewModel we setup all initial values on the properties, and create the `ReactiveCommands` and subscribe to them.

Also we create the `GithubAuthenticationActor` passing the ViewModel as an argument to the actor. 

The idea is to use the actor as a service and use the ViewModel to bind to the form and to the actor. The actor can update the ViewModel and WPF will make sure that changes to normal properties are updated in the right thread. 

Note: It appears that when we create the `GithubAuthenticationActor` it is looking into the App.Config and it uses the `synchronized-dispatcher` setup in the HOCON. 

The only changes to `GithubAuthenticationActor` are the injection of the ViewModel in its constructor, and updating the ViewModel properties for `Status` and `StatusLabelColor`. When we get authenticated then the actor calls into the ViewModel to tell it we are authenticated and the ViewModel will continue on.


### Step 2 - Setup LauncherForm.XAML

The `LauncherForm` creates the viewmodel and binds to it. Also it hooks into the `Window_Closing` event to shutdown the system when it closes.

The ViewModel called `LauncherFormViewModel` sets up some properties to bind the form with:

1. `RepoUrl` property that binds to the TextBox, where we tell it which repository to scan.
2. `Status` property that binds to the StatusBar on the form.    
3. `StatusForeColor` property that binds to the color of the status message.
4. `LaunchSearch` command to bind to the button on the form.

In the constructor of the ViewModel we create all actors needed for the search. The `MainActorForm` takes the ViewModel as a parameter, we create the `GithubCommanderActor` and the `GithubValidatorActor` as before. We create the `ReactiveCommand` to launch the search, which tells the `MainFormActor` to start processing the repository.

The `MainFormActor` will use the ViewModel to update the status message and color and will launch the `ResultsWindow` by calling into the ViewModel LaunchResults method. 
`LaunchResults` will call the `OnNext` on the LaunchWindow `Subject` (ReactiveUI) to launch the `RepoResultsForm`. All this is done on the UI thread because we are observing on the dispatcher when we subscribe to the `LaunchWindow` subject. This is all possible because of the `Reactive UI` component.

### Step 3 - Setup LauncherForm.XAML

The `RepoResultForm` displays all the repositories found in a `ListView`, it also creates the viewmodel and binds to it. Also it hooks into the `Window_Closing` event to stop the `RepoResultsActor`.

The ViewModel called `RepoResultsViewModel` sets up some properties to bind the form with:

1. `Title` property that binds to the title of the form.
2. `Items` property that binds the ListView as a `ReactiveList`.    
3. `Status` property that binds to the StatusBar on the form.    
4. `ProgressValue` property to keep the count of users thus far.
5. `ProgressMax` property to keep the number of expected users.

In the constructor of the ViewModel we create the `RepoResultsActor` with a synchronized dispatcher because WPF needs to be on the UI thread to update the list.

The `RepoResultsActor` will use the ViewModel to update the status when it receives the `GithubProgressStats` message. And will update the `Items` on the ViewModel when it receives the `IEnumerable<SimilarRepo>` message.


And with that, you're all set! 

We have a model to use Akka.Net with WPF and Reactive UI, and MahApps.Metro.

## Any questions?
**Don't be afraid to ask questions** :).

Come ask any questions you have, big or small, [in this ongoing Bootcamp chat with the Petabridge & Akka.NET teams](https://gitter.im/petabridge/akka-bootcamp).


### Problems with the code?
If there is a problem with the code running, or something else that needs to be fixed in this project, please [create an issue](https://github.com/njimenez/AkkaProjects/issues) and we'll get right on it. This will benefit everyone.

