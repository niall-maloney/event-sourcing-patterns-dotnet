# Single Current Aggregate

The single current aggregate or open/close book pattern in event sourcing is all about managing periods of activity with
clear start and end points. When you "open the book," you start a new period of activity, and when you "close the book,"
you wrap up that period, often summarizing what happened during that time. This pattern helps keep things organized and
makes it easy to see what happened when. It also solves the problem of never ending aggregates that can have infinite
events in their stream. 
