# GamePack & Boilerplate Documentation

# 1. Boilerplate

## Main

- **Object Extensions**
    
    ```csharp
    public static T InstantiateInLevel<T>(this T obj, Vector3? position = null, Quaternion? rotation = null)
    public static T MoveToLevelScene<T>(this T obj)
    ```
    

## Structure

- **Controller**
    
    ```csharp
    protected virtual void OnLevelDidStart(TMainSceneRefBase mainSceneRef, TLevelSceneRefBase levelSceneRef)
    protected virtual void OnLevelDidStop()
    ```
    
- **View**
    
    ```csharp
    public bool IsVisible
    public void DestroyView()
    public void SetIsVisible(bool isVisible)
    public void LogInWorld(object msg)
    ```
    
- **PhysicsTrigger [[EXAMPLE]](../Runtime/Examples/PhysicsTrigger/PhysicsTriggerExample.cs)**
- 

    ```csharp
    public void ListenFor<T>(PhysicsTriggerEvent<T> action, PhysicsEventPhase? phase = null)
    
    // Usage
    _SomeTrigger.ListenFor<MeshRenderer>(DidTriggerWith, PhysicsEventPhase.Enter);
    ```
    
    ```csharp
    public enum PhysicsEventPhase
    {
        Enter, Stay, Exit
    }
    ```
    

# 2. Tools

### [Systems](../Runtime/Tools/Systems)

- **DebugDrawSystem -** Draw shapes in world space for debug purposes. **[[EXAMPLE]](../Runtime/Examples/DebugDraw/DebugDrawExample.cs)**
- **ParticlePlayerSystem -** Play particles globally. **[[EXAMPLE]](../Runtime/Examples/TimerExample/TimerExample.cs)** **[[EXAMPLE]](../Runtime/Examples/TimerExample/RepeatingTimerExample.cs)**
- **TimerSystem** - Define and execute timed actions.

### **[Behaviours](../Runtime/Tools/Behaviours)**

- **BasicGravity** - Basic gravity simulation.
- **FixedRotation** - Keep rotation fixed, independent of parent rotation. Can be used to anchor rotation of child objects.
- **FollowObject** - Follow a target object smoothly.
- **LookAt** - Look at target object.
- **RandomGraphics** - Select and enable only one of a game object in an array.
- **RotateAround** - Rotate around at a fixed speed.
- **RotateWithCamera** - Fix an objects rotation as same as the specified camera.
- **Exploder** - Explode an array of objects with an ability to rewind later.
- **FollowingParticleSystemShape** - Set particle system shapes position as the target object.
- **ScatterObjects** - Scatter objects at a position in a radius.
- **StaggerringForwardMovement** - Forward movement with staggering ability.
- **StrayingParticles** - Control particle positions with straying behaviour.
- **BoneTargeter, BoneTargeterGroup** - Used to update directions of bones by defining them in a group. Targeting has parameters to control rotation priority of bones and amount to rotate.
- **AnchorToPoint -** Used to stabilize an object with an anchor. Can be used with animation.

### **[Helper](../Runtime/Tools/Helper)**

- **ChangeDetection** - Detect a change and run a callback. Detection can be delayed.
- **ConstantChangeDetection** - Detect a change constantly.
- **PolyLinePath** - Path with multiple lines. Can use **PolyLinePathFollower** to snap and move along the path. **[[EXAMPLE]](../Runtime/Examples/PolyLinePath/PolyLinePathExample.cs)**
- **Sleep -** Define a timer to check later.
- **TransformInfo** - Create with a transform with an ability to set values at creation later.
- **PrefabContainer -** Helper to reduce scene size when using too many prefabs.

### [Input](../Runtime/Tools/Input)

- **InputDragHandler** - Listen for pointer drag amount.

### [Modules](../Runtime/Tools/Modules)

- **Minimap** - Show object positions on a UI panel. **[[EXAMPLE]](../Runtime/Examples/Minimap/)**
- **OffScreenIndicator** - Show object directions on a screen edges when they are offscreen.
- **SlingCar** - Forward moving object with slinging motion used to predefined lanes. **[[EXAMPLE]](../Runtime/Examples/SlingCar/)**
- **SnapDrag** - Drag and snap an object to a designated position.
- **StateMachine** - Basic state machine implementation that can be extended and used with a custom enum as state. **[[EXAMPLE]](../Runtime/Examples/BasicStateMachine/StateMachineControllerExample.cs)**

### [Visual](../Runtime/Tools/Visual)

- **ObjectFader** - Fade in/out objects with a duration.
    - Fader
    - MeshFader
    - SpriteFader
    - UIFader
- **ComboController** - Detect combos and show feedback.
- **Countdown** - Show a countdown text.
- **SegmentArray** - Array of objects in a straight line.
- **SplitHpBar** - Hp bar with split graphics for UI.

# 3. Logging

- **ManagedLog**

```csharp
public static void Log(object obj, Type type = Type.Default, Object context = null, Color? color = null, bool avoidFrameCount = false)
public static void LogError(object obj, Object context = null)
public static void LogMethod(object obj = null, Type type = Type.Default, Object context = null, Color? color = null, int stackOffset = 0)
```

- **WorldLog [[EXAMPLE]](../Runtime/Examples/Logging/LoggingExample.cs)**

```csharp
public static void Log(object msg, Vector3? pos = null, Transform localTransform = null, Color? color = null)
public static void OnScreen(object msg, Color? color = null)
```

# 4. Custom Attributes

```csharp
[AutoFillSelf]
[AutoFillChildren]
[AutoFillScene]
[RenameInHierarchy]
```

# 5. Utilities

- **CoroutineRunner** - To run coroutines in classes other than MonoBehaviour.
- **EaseCurve** - Helper class that can define a custom curve or an easing function. Can be edited in Editor.
- **Extensions** - Several extensions to help with common operations.
    
    ```csharp
    // Transform
    public static void SetGlobalScale (this Transform transform, Vector3 globalScale)
    // Range (Vector2)
    public static float GetRandomValueAsRange(this Vector2 v2)
    // Collection
    public static void ForEach<T>(this IEnumerable<T> coll, Action<T> func)
    public static int GetIndexWithMaxValue<T>(this IEnumerable<T> coll, Func<T, float> func)
    public static int GetIndexWithMinValue<T>(this IEnumerable<T> coll, Func<T, float> func)
    public static T GetRandom<T>(this T[] array)
    ```
    
- **FindAllObjects** - Unity’s FindObject doesn’t return inactive gameobjects, this does.
- **InterfaceExtensions** - ****GetComponent for interfaces**.**

# 6. Vendor

- **LeanTween**
- **Colors** - Cool colors to use instead of default Color.red etc.
- **CustomZTestUI** - Used to override sorting for UI elements.
- **EasingFunctions** - Collection of easing functions.
- **SROptions.Custom -** Copy this in project to use SRDebugger.