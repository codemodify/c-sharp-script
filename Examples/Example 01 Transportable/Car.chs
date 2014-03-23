
using System

class Car : CategoryBTransportable

      Init()
          Length = 3
          Width = 1.5

      About()
          base.About()
          Console.WriteLine( "And I'm a Car" )

      Move( int distance )
          Console.WriteLine( "Moving on 4 wheels." )