## Release 1.0

### New Rules

Rule ID | Category | Severity | Notes
--------|----------|----------|--------------------
SSCN001  |  Usage  | Error | `[SingletonAttribute]` can only be applied to classes that derive from one of the following base types: `MonoBehaviour`, `ScriptableObject`.
SSCN002  |  Usage  | Error | `[SingletonAttribute]` can only be applied to sealed partial classes.
SSCN003  |  Usage  | Error | `[SingletonAttribute]` cannot be applied to generic classes.
SSCN004  |  Usage  | Error | A singleton class must use `BeforeAwake` or `AfterAwake` methods instead of `Awake()`. They are called at the beginning of `Awake()` and at the end of `Awake()`, respectively.
SSCN005  |  Usage  | Error | A singleton class must use `BeforeDestroy` or `AfterDestroy` methods instead of `OnDestroy()`. They are called at the beginning of `OnDestroy()` and at the end of `OnDestroy()`, respectively.