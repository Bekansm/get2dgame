using System;

public struct PlayerData : IEquatable<PlayerData> {
    public ulong clientId;
    public string playerName { get; private set; }

    public bool Equals(PlayerData other) {
        return clientId == other.clientId && playerName == other.playerName;
    }

}
