# Fix for Boolean Options Not Working

## Problem
Boolean options (like `--parallel`, `--verbose`, `--dry-run`) were not being recognized or processed correctly. The `RunClass` method was only handling parameters marked with `ArghParameterAttribute` (arguments) and completely ignoring parameters marked with `ArghOptionAttribute` (options).

## Root Causes

### 1. Options Were Not Being Added to Commands
The original `RunClass` method only processed parameters with `ArghParameterAttribute` and added them as arguments. It never checked for `ArghOptionAttribute` or added any options to commands.

### 2. ProcessParameters Helper Was Not Being Used
There was already a `ProcessParameters` helper method that correctly handled both arguments and options, but it was never called by the `RunClass` method.

### 3. Incorrect Value Retrieval
The `GetValue` methods were trying to retrieve values using string names instead of the actual `Symbol` objects (Option/Argument), which is required by `System.CommandLine`'s `ParseResult` API.

### 4. DBNull Handling
The default value creation logic didn't handle `DBNull` values, causing type conversion exceptions.

## Changes Made

### 1. Updated `RunClass` Method
Changed the method to use the existing `ProcessParameters` helper, which properly classifies parameters as either arguments or options and creates the appropriate symbols.

### 2. Fixed Value Retrieval
- Updated `GetOptionValue` to accept an `Option` object instead of a string name
- Updated `GetArgumentValue` to accept an `Argument` object instead of a string name  
- Modified `ProcessParameters` to pass the actual symbol objects to the value getters

### 3. Enhanced `GetValueBinder<T>`
Updated to have two overloads:
- `GetValue(ParseResult, Option<T>)` for options
- `GetValue(ParseResult, Argument<T>)` for arguments

This properly calls `ParseResult.GetValue()` with the strongly-typed symbol objects.

### 4. Fixed DBNull Handling
Added checks for `DBNull` in both `CreateOption` and `CreateArgument` methods to prevent type conversion exceptions.

## Testing
The fix allows boolean options to work properly:

```bash
# These now work correctly:
Argh.Sample.exe test ./tests --parallel
Argh.Sample.exe test ./tests -p                        # Using alias
Argh.Sample.exe build ./solution --verbose
Argh.Sample.exe deploy production --dry-run
```

All option types (bool, int, string) now work correctly with both full names and aliases.
