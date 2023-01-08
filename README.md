# com.bustedbunny.mvptoolkit

Model-View-Presentation Toolkit for using with Unity Entities.

## How to use:

### Creating basic screen

*It's not important when you create UXML document for your screen*

1. Create a presentation class and it's authoring

```
    public class MainMenuAuthoring : UIAuthoringBase<MainMenuPresentation>
    {
    }
    public class MainMenuPresentation : BasePresentation
    {
    }
```

2. Now create a game object in SubScene and add your authoring component to it.
   In authoring component inspector add your uxml asset into proper field.
3. Now start the game and as soon SubScene is loaded,
   your UI screen will be loaded and ready for usage.

### Opening screens

1. In order to open your screen you need to define url on your screen first.
   You can do it as example below.

```
    [StateURL("OpenMainMenu")]
    public class MainMenuPresentation : BasePresentation
    {
    }
```

2. Then somewhere in your code call the next code:

```
EntityManager.SetStateRequest(StateType.Strong, "OpenMainMenu")
```

Now let's breakdown what would this do:

* StateType.Strong means all other screens if they were open will be closed.
* "OpenMainMenu" means that all screens/callbacks with this URL will be called on next frame.

### Making custom callbacks

Now if want to just make some custom changes in UI without
involving opening/closing any screens, we can do so this way:

```
    [StateURL("OpenMainMenu")]
    public class MainMenuPresentation : BasePresentation
    {
        [StateURL("DoCustomCallback")]
        private void DisableOnOtherMenuOpen()
        {
            Debug.Log("Made custom callback");
        }
    }
```

That means if some code make a request as below, this method will be executed:

```
EntityManager.SetStateRequest(StateType.Soft, "DoCustomCallback")
```

This is the difference between `StateType.Strong` and `StateType.Soft`:

* Any `Strong` will close all active screens.
* `Soft` will simply make a requested callback.
  That means you can also open other screens simultaneously.