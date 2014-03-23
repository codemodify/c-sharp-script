
### What is it
C# Script is a practical and elegant scripting language that resembles a simplified C# syntax that is easy and intuitive. 
This is a R&D work in progress. Supported features include the ones that are supported by the .NET version you choose. 

### The Vision
The vision behind this project comes from different situations when
 * a scripting scenario is more welcome
 * you want to boost the developpement time
 * need to extend the app's functionality

### The elegancy behind it
To be stable and be less in the dev-cycle for this project, we would want to be based on the 
powerfull facility of the .NET without reinventing the wheel. The approach is simple: use a simplified syntax 
that is no less powerfull than C#'s , parse the source files, add the missing stuff to build a valid C# source file, 
use the .NET CodeDom to generate assemblies on the fly. 

### Features
 * VisualStudio AddIn / Extension to be able to mix and compile ".chs" files within the solution
 * The CSharpScript assembly as a library

### Example
![](https://raw.github.com/ukoreh/c-sharp-script/master/example.png) 
