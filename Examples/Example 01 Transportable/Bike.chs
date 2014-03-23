
using System

class Bike : CategoryATransportable

      Init()
          Length = 1
          Width = 0.2

      About()
          base.About()
          Console.WriteLine( "And I'm a Bike" )

      Move( int distance )
          Console.WriteLine( "Moving on 2 wheels." )

      Stop()
          Console.WriteLine( "Trying to stop my powerful engine..." )
          base.Stop()
