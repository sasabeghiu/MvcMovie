## Movie API
### This repository is used for a internship entry assignment.

### Assignment
```
Create a simple website on which you can search for a movie data with the criteria:
  a. Use an API of an online movie database (IMDB) to retrieve information;
  b. Show results on page;
  c. Use C# on .NET Core;
  d. Use an API of an online service (YouTube) to show a trailer in the results;
  e. Make sure website is secure and fast.
```

### To take in consideration
```
The YouTube API has a limit of 10.000 queries per day, and if the limit is reached, it will 
throw an GoogleAPIException (the request cannot be completed, because you have exceeded your quota).

Since the IMDB API returns 10 results per search, the YouTube API is used 10 times per search.
However, after searching for 4-5 times the daily limit will be reached. Then the results page 
will have an error untill the next day.

If that is the case, the MovieController has another method (currently commented out) that 
parses the movie result without a trailer. This method (between lines 46-61) can be commented out 
and used when daily limit has  been reached. Of course, the other method that parses the movie with 
a trailer should be commented out.

If the method ParseMovieData parses the movie with a trailer, use var model =  await ParseMovieData(result);
If the method ParseMovieData parses the movie without a trailer, use var model =  ParseMovieData(result);
```
