# About ManOCL #

One person has sent me a letter asking about how it compares to other frameworks such as CLOO and other ones, how complete is ManOCL, so this post describes current state of things in ManOCL

**Comparisons:**
Actually I wasn't doing any comparison tests with other frameworks Shan, ManOCL is very young framework, its to early to compare muscles. It's primary aim is to provide clean, fault tolerant and transparent OOP model that is easy to integrate to existing project structure and to existing team of developers that know nothing about OpenCL. Current transparency level of ManOCL allows people with zero knowledge of OpenCL to start coding real kernels in a matter of minutes, forgetting about contexts and other advanced stuff that will become needed after a two years of development on a state of art optimized project. In my case when I started learning OpenCL it took me a week to create my own sample project in C#, the code was a totally unmaintainable 1km long spaghetti and minimal changes followed with hours of debugging, nightmare... After that I've started looking to existing frameworks OpenCL.NET, OpenTK Cloo, none of them were good and posed additional integration problems, so I've developed my own.

**Regarding functional coverage:**
When I was developing ManOCL my intention was to hide advanced aspects of OpenCL, things that allow you to gain 0.7% of performance boost with additional 1000 lines of code are not directly available, all of these features are available via native interop which has 100% coverage. The amount of current apis is enough to create complete application that uses  OpenCL and interoperates with OpenGL via OpenCL/OpenGL interop, which I think is 95% of all cases. If you see that ManOCL is lacking some functionality or has some other problems just e-mail me, I will try to help as much as I can.

**The amount of non C# coding:**
Well still you need to create your kernels in OpenCL :) I'm using ManOCL in my projects and they are 100% C#