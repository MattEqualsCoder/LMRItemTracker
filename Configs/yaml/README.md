# Configs

This documentation covers the various [YAML](https://www.cloudbees.com/blog/yaml-tutorial-everything-you-need-get-started) config files which are used for the voice tracking functionality.

## Config Files

There are five config files which can be modified for tracker lines:
- **items.yml** - Includes the various items which can be picked up, the names for tracker to say when picking them up, and special lines for when items are tracked.
- **locations.yml** - Includes the various locations and their names. Used for hints.
- **regions.yml** - Includes the various regions and their names. Used for hints.
- **responses.yml** - This is for generic lines for tracker to respond with for built in functionality.
- **twitch.yml** - This is configs for Twitch chat functionality
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


### Twitch Prediction Polls
Any number of Twitch prediction polls can be added, each with different prompts to trigger the poll, options for people to choose from, and responses when the prediction poll has been closed.

**Fields**:
- StartPrompts (List of Strings): The prompts to initiate the prediction poll (without "Hey Tracker" added to the beginning)
- ResolveGoodPrompts (List of Strings): The prompts to resolve the prediction poll with the "good" result (without "Hey Tracker" added to the beginning)
- ResolveBadPrompts (List of Strings): The prompts to resolve the prediction poll with the "bad" result (without "Hey Tracker" added to the beginning)
- StartResponses (Weighted Possibilities): The list of possibilities for when tracker starts the prediction poll
- PredictionTitles (Weighted possibilties): The list of titles to show up on the Twitch prediction polls
- PredictionOptions (Custom - see example below): The list of possible options for people to select from. (Max 25 characters.)
- ResolvedResponses (Weighted possibilties): The list of possibilities for when tracker resolves the prediction poll
  - {0} is replaced with the winning choice
  - {1} is replaced with the number of people who voted for the winning choice
  - {2} is replaced with the number of points spent on the winning choice
  - {3} is replaced with the name of someone who voted on the winning choice
  - {4} is replaced with the number of people who voted for the losing choice
  - {5} is replaced with the number of points spent on the losing choice
  - {6} is replaced with the name of someone who voted on the losting choice

In the below example, saying "Hey tracker, the trials fairy has something" will trigger Tracker to open a prediction poll for 2 minutes. The Twitch prediction poll with have the title "Does the trials fairy have the goods?" with either "It has the goods!" and "It has trash" or "It'll be great!" and "It'll suck!" as the two options people can select.

Once the outcome has been discovered, the prediction poll could be resolved by saying "The trials fairy had the goods" or "The trials fairy had trash". Tracker will then close the prediction poll and pick one of the resolved responses to say, substituting the above values in for the placeholders.

Note if you ever need to end a Twitch prediction early, you can either say "Hey tracker, lock the prediction poll" to tell her to close it so that people can't vote anymore or say "Hey tracker, terminate the prediction poll" to close it and refund the points back to people.

**Example**
```
Predictions:
  - Key: "Trial Fairy"
    StartPrompts: 
      - "the Trials fairy has something"
    ResolveGoodPrompts:
      - "the trials fairy had the goods"
    ResolveBadPrompts:
      - "the trials fairy had trash"
      - "the trials fairy had nothing"
    StartResponses:
      - Text: "Well then. Chat, a poll will be open to predict if the trials fairy will be a worthwhile endeavor. Good luck, I personally think this will be a waste of time."
      - Text: "Well then. Chat, a poll will be open to predict if the trials fairy will be a worthwhile endeavor. Good luck, I choose to believe."
      - Text: "Alright. Chat, a poll will be open to predict if the trials fairy will be a worthwhile endeavor. I already know the answer, so good luck to the rest of you."
      - Text: "Let's go. Chat, what do you think? Are we wasting time or is this progress. Poll should now be open for 2 minutes"
      - Text: "Time to get blue balled by a lizard. Chat, poll should now be open for 2 minutes, do you have faith or is this a waste. Good luck!"
    PredictionTitles:
      - Text: "Does the trials fairy have the goods?"
    PredictionOptions:
      - Good: "It has the goods!"
        Bad: "It has trash"
      - Good: "It'll be great!"
        Bad: "It'll suck!"
    ResolvedResponses:
      - Text: "Well I sure bet {6} feels silly they didn't pick the other option"
      - Text: "Oh look, {5} channel points were wasted on that. Too bad."
      - Text: "Congrats to {3} for getting it right. This time."
      - Text: "Wow. A total of {1} people got it right."
  ```