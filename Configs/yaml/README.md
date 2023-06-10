# Configs

This documentation covers the various [YAML](https://www.cloudbees.com/blog/yaml-tutorial-everything-you-need-get-started) config files which are used for the voice tracking functionality.

## Config Files

There are five config files which can be modified for tracker lines:
- **items.yml** - Includes the various items which can be picked up, the names for tracker to say when picking them up, and special lines for when items are tracked.
- **locations.yml** - Includes the various locations and their names. Used for hints.
- **regions.yml** - Includes the various regions and their names. Used for hints.
- **responses.yml** - This is for generic lines for tracker to respond with for built in functionality.
- **custom.yml** - This is for adding in custom prompts and responses for tracker to say.

## Editing Config Files

Config files can go in one of two locations: side-by-side with the executable or in %localappdata%\LMRItemTracker. When launching, it'll load the config files automatically. If any changes are made to the config files, you'll need to restart the tracker application.

For editing, I'd recommend downloading Visual Studio Code.

**Notes About Editing**
- Yaml files are sensitive about white space at the beginning of lines as it uses that white space to determine what is part of the same list of data.
- Strings in Yaml files don't need quotes around them, but Yaml doesn't like some characters at the beginning of strings unless you have quotes around the lines. For example, it doesn't like starting lines with {0}. Because of this, it's recommended to enclose all strings in quotes.
- If you run into problems with a Yaml file loading, you can try running it through an online validator such as [this one](https://www.yamllint.com/), but it won't pick up all errors.

## Property Types

### **String**
This is the basic type of property. It's basically just a single word or phrase. Mostly used for keys and things which shouldn't be modified.

**Example**:
```
SpoilerFileName: "Flail Whip"
```

### **Item Type**
This is used to distinquish the importance of items for hint purposes. The following is the list of item types:

- Useless
- NiceToHave
- NiceToHaveChecks
- PotentiallyProgression
- Progression
- AnkhJewel

Currently there is no difference between NiceToHave and NiceToHaveChecks.PotentiallyProgression, Progression, and AnkhJewels are treated equally as well.

**Example**:
```
Type: NiceToHave
```

### **List of Strings**
Pretty self explanatory. This is a list of various texts that don't need to be weighted, such as the list of hints which go in order from first to last. Basic lists are denoted by a series of lines starting with a hyphen. **Note**: The indention of the hypens need to match. Unlike the previous two types, the name of the property goes in the line before the first item in the list.

**Example:**
```
Hints:
- This is the first hint
- This is the second hint
```

### **Weighted Possibilities**
These are sort of similar to the list of strings, however a weighted possibility has two properties to it: the text, and the weight. The text is the string that tracker would say. The weight is a number that represents the likelihood of a line being said. The higher a weight is in comparison to the weights of the other possibilities, the more likely it will be to be said.

Note that for weighted possibilities, you'll need to include "Text: " in front of the string you want Tracker to say, and if you include a weight it'll be on the following line with "Weight: ". Do not include a hyphen in front of weight, and make sure it matches the indention of the capital T of Text. (This will make more sense in the example below). The weight is also optional. If none is included, it is assumed to be 1. Weights can also be decimal numbers.

The weights work by totaling all of the weights of the possibilities, then each possibility will have the liklihood of being said based on its percent of the total. In the following example, the total of all weights is 12. The option with a weight of 6 will be said 50% of the time, the option with a weight of 4 will be said 33% of the time, and the final option with a weight of 2 will be said 16.5% of the time.

**Example:**
```
Names:
- Text: "Name 1"
  Weight: 6
- Text: "Name 2"
  Weight: 4
- Text: "Name 3"
  Weight: 2
```

### **Rollup Responses**
These are for things that can have different sets of possible responses based on the number of times something has happened. For example, tracker giving sassier lines when you've died over five times in comparison to you dying the first time.

For this, you'll include a number on one line (followed by colon), then underneath it you'll have the weighted possibilities for that number. If you skip numbers, the responses for a number isn't listed in the config file will be rolled up to the next highest number less than it. In the below example, since 3 and 4 aren't listed, the prompts for 2 will be used.

Be mindful of indentation here. The possibilities under a number need to be indented further than than the number.

**Example**
```
PlayerDied:
  1: 
    - Text: "Wow, only your first death?"
  2:
    - Text: "You now have somewhere between 2 to 4 deaths"
    - Text: "This line will also be possible for between 2 to 4 deaths, but it's less likely due to the weight."
      Weight: 0.1
  5:
    - Text: "You now have at least 5 deaths"
```
