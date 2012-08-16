This is a tower defense game I built as a "do it in a week" challenge, and the
result is fairly final (the week is over), except for bugfixes or whatever.  But
it would be cool if someone wants to fork it and do something interesting!

GAME
----

The tower defense is a "build your own maze" style, in the tradition of Desktop
Tower Defense and its many imitators, with a slight change that you're digging
out a maze instead of blocking in one.  Thus, your space is only limited by your
money (and perhaps your RAM).

The game itself is a little feature-light; there are only the four classic types
of enemies (basic, fast, tough, and immune) but each has strengths and weaknesses,
forcing the player to balance the tower types in their maze or be overrun.

CODE
----

The code is written in C# (using XNA 4.0), both of
which are freely available through the DreamSpark program (as of time of writing)
for anyone with a .edu email address.

Documentation is throughout, in the triple-slash C# style that VisualStudio
encourages, and I believe the code is fairly self-documenting.  In any case, it
shouldn't be too difficult to dive in and start making changes.

GRAPHICS
--------

Such as they are.  I drew them with [GraphicsGale][], which is a pretty good tool if
you're hoping to make lots of 30x30 images without losing your mind.  That's what
all the .gal files are.

   [GraphicsGale]: http://www.humanbalance.net/gale/us/

I added the transparencies in with the Gimp, which is a powerful but extremely
quirky program, even on its native Linux (add some instability if you happen to
be using, say, Windows).

ETC
---

This game copyright Richard Rast, who releases all rights for any non-commericial
purpose, with or without attribution, to anyone, for any reason.  No warranty of
any kind is given for any part of this software, but it worked fine on my machine.
