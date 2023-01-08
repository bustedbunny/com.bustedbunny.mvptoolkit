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

2. Now create a game object and add your authoring component to it.
   In authoring component inspector add your uxml asset into proper field.
4. Now start the game and your UI screen will be loaded and ready for usage.

### Opening screens

1. In order to open your screen you need to define url on your screen first.
3. You can do it as example below.

*You may also not define url, then type name excluding `Presentation` word will be used.*

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

### State URL Browser Editor Tool

Now if you want to inspect all existing URL in current build as below -
open custom Editor Window in `Tools/State URL Browser`

![img.png](img.png)

### Localization Support

In order to localize your UI you will need to use Unity Localization Package.

Simply create String Tables using this package and assigns matching table
to your UI in your authoring component, or leave it blank if you don't need it.

![img_1.png](img_1.png)

### Smart String Localization Support

In order to bind your `TextElement` to Locale you need:

1. Assign binding in inspector as follows:

* `Text` property must contain key of your localized string with `#` prefix.
* `BindingPath` property must contain string identifier of bound property.
  Multiple properties can be split with dot, for example: `WorkerCost.WorkerValue`.

![img_2.png](img_2.png)

2. In `OnInit` method of your `BasePresentation` class
   assign `propertyProvider` field.
   You can use default `PropertyProvider` class.

```
            propertyProvider = new PropertyProvider();
```

3. Fill it with your own `BindableProperty`
   objects using `IPropertyProvider[string binding]` setter.

`BindableProperty` can be created this way.

```
            private readonly BindableProperty _workersCount = new();
```

4. Now you can assign values using `BindableProperty.Value` property.
   All formatting will be assigned automatically, using events.

### Rebinding Key of TextElement

If for some reason you want to bind your `TextElement` to another key
use `BasePresentation.UILocalization.Rebind` method.

This will properly dispose previous bindings and assign new one to
specified key.