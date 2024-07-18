# RefhackCasts
Having to wait for a PrimitiveMemberEditor sync got you down?

This is a ResoniteVR mod that makes refhacking easier by modifying ObjectCast<Object, Object> (an otherwise useless cast) to toggle between RefID and ulong (and for anything else, act like before)

This lets you loop through multiple RefIDs and grab the corresponding IWorldElement immediately, in the same execution context
