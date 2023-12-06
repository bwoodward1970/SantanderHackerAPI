# SantanderAPICodeTest

Caveats below due to time. Would be addressed in true dev/production environment.

Need to improve the startup time, should retrieve as soon as the first story is added.

Performance/Memory improvements to Processing HackerNews API Calls, use system.threading.channels or similar to 
create a pipeline for processing each story. Linked with using streams over grpc from firebase. I did spend some
time looking into this however the firbase libraries aren't complete for using C# with the realtime database api.

Better Null Checking

Better Comments

Full Test Coverage

Error Handling 

Better Logging


