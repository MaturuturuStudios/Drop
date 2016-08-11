using System;

public struct LevelInfo {
        public int world;
        public int level;

    public static bool operator ==(LevelInfo a, LevelInfo b) {
        if (((object)a == null) || ((object)b == null)) {
            return false;
        }
        return a.world == b.world && a.level == b.level;
    }

    public static bool operator !=(LevelInfo a, LevelInfo b) {
        return !(a == b);
    }

    public override bool Equals(Object obj) {
        return obj is LevelInfo && this == (LevelInfo)obj;
    }
    public override int GetHashCode() {
        return world.GetHashCode() ^ level.GetHashCode();
    }
}
