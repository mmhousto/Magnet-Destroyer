This is a tool to simulate magnetism in unity, you just have to add the script to the object and it will be endowed with mangetism.

If you are using rigidbody 2D and Colliders 2D use the 2D version.

The script allows you several options that we will explain below:

-IsMetallic: makes the object be attracted to other magnetic objects but it has no magnetism.

-Is static: if the object is activated it will not move from the site but it will have magnetism and will be affected by other fields even if it does not move.

- Magnetism without use forces: it is not recommended to activate it since then the script will stop using forces and the effect will be less real but basically what it does is stop using the unity physics engine and simulate magnetism through calculations.

- Magnetic Rigidbody: it is the link to the rigidbody itself, it helps us if we want the mass to influence the magnetic force, if we do not put it, it will be assumed that the mass is one Kg.

- Advance settings: Unlocks advanced options such as the time the magnetism is off and on or the time the magnet changes polarity, this option allows us to do puzzles and challenges more easily.

- Turn On Magnetism: This option is true by default, if we remove it, the magnetism of the object will not affect anyone.

- Affect by Magnetism: This option is true by default if we change it, the object will not be affected by any magnetism.

- North Pole: With this option we choose which will be the polarity of the object.

- Constant magnetic Force: If we activate this option we eliminate the reduction of force with distance so that the magnetic field will remain with the same force throughout the affected space.

- Magnetic Force: It is the power of the force that the object has, although the mass of the object also influences we can make more powerful magnets but weigh the same.

- Magnetic Distance: It is the distance in which the magnetic field acts, which can be seen in the editor thanks to a gizmo.

- Axis block movement: blocks the axis in which that object can be moved by magnetism.

- Allow Tags: if we introduce object tags in the list, only those objects will be affected by magnetism, if we leave it empty, all objects with the tool will be affected.