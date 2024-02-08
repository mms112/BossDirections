===== ABOUT THE MOD =====

This mod allows you to offer boss-related items to any fire to get pointed to that boss nearest altar location.

This works SIMILAR like interacting with a vegvisir.

If your config is set to pinless=true (which is the default), the offering will work as 'interacting with a vegvisir in no-map mode', that is, the camera will rotate and point you to the boss location (regardless of if you're actually playing in no-map mode).

If you mess with config file and set pinless to false, it would work EXACTLY lke interacting with a vegvisir - that means it'll also pin the boss location on your map, which can be problematic if you're running a no map game using cartography mods and stuff. So my advice is to NOT change that config.

===== HOW TO USE IT =====

Put the item on your top hotbar (1-8) and 'use' it on the fire, like when you use wood to add fuel to it. Do it with your mouse 'free' so the camera can move to where it should. 

===== CONFIGURATION =====

There is a json file in the mod folder. It has a list of locations that can be discovered, alongside a list of possible offerings to get their info. There is also some flavor text that I added to make it more interesting. You can change anything there if you want, just make sure you add the right item codes (that can be different from their spawn-command ids).

===== OFFERINGS =====

By default, you can find those location offering those items:

BOSSES:
- Eikthyr (deer trophy x1)
- Elder (ancient seed x1)
- Bonemass (withered bone x10)
- Moder (dragon egg x1)
- Yagluth (goblin totem x1)
- Queen (seeker brute trophy x1)

VENDORS:
- Haldor (coins x100)
- Hildir (ruby x5, or amber x20, or amber pearl x10)

OTHERS:
- Stone Altar (the start location) (stone x10)

===== COMPATIBILITY ISSUES =====

If you're using the "Useful Trophies" mod, you probably won't be able to offer the deer trophy to the fires. My recomendation in that case is that you change your offerings.json file and add another item option for Eikthyr, like "deerhide": 5.