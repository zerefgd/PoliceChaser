using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Constants
{
    public class Settings
    {
        public const string SETTINGS_MUSIC = "SETTINGS_MUSIC";
        public const string SETTINGS_SOUND = "SETTINGS_SOUND";
        public const string SETTINGS_SHAKE = "SETTINGS_SHAKE";
    }

    public class EventNames
    {
        public const string GAME_START = "GAME_START";
        public const string GAME_OVER = "GAME_OVER";
        public const string UPDATE_SCORE = "UPDATE_SCORE";
        public const string MISS = "MISS";
        public const string SPAWN_COIN = "SPAWN_COIN";
        public const string UPDATE_COIN = "UPDATE_COIN";
        public const string UPDATE_ITEM = "UPDATE_ITEM";
        public const string UPDATE_HEALTH = "UPDATE_HEALTH";
        public const string REFRESH_SCORE = "REFRESH_SCORE";
        public const string REFRESH_COIN = "REFRESH_COIN";
        public const string UPDATE_STORE_ITEM = "UPDATE_STORE_ITEM";
    }

    public class Data
    {
        public const string SCORE = "SCORE";
        public const string COIN = "COIN";
        public const string CAR = "CAR";
        public const string BOOST = "BOOST";
        public const string REVENGE = "REVENGE";
        public const string HEALTH = "HEALTH";
    }

    public class Tags
    {
        public const string PLAYER = "Player";
        public const string ENEMY = "Enemy";
        public const string COIN = "Coin";
        public const string MAGNET = "Magnet";
        public const string SHIELD = "Shield";
        public const string BOMB = "Bomb";
    }

    public class ScoreMessage
    {
        public const string TYPE = "TYPE";
        public const string NORMAL_COLLISION = "NORMAL";
        public const string MISS = "MISS";
        public const string REVENGE = "REVENGE";
        public const string POSITION = "POSITION";
        public const string AMOUNT = "AMOUNT";
        public const string INCREASE = "INCREASE";
        public const string BUY = "BUY";
    }

    public class Store
    {
        public const string CAR = "CAR";
        public const string TRAIL = "TRAIL";
        public const string CAR_1 = "CAR_1";
        public const string CAR_2 = "CAR_2";
        public const string CAR_3 = "CAR_3";
        public const string CAR_4 = "CAR_4";
        public const string CAR_5 = "CAR_5";
        public const string CAR_6 = "CAR_6";
        public const string TRAIL_1 = "TRAIL_1";
        public const string TRAIL_2 = "TRAIL_2";
        public const string TRAIL_3 = "TRAIL_3";
        public const string TRAIL_4 = "TRAIL_4";
        public const string TRAIL_5 = "TRAIL_5";
        public const string TRAIL_6 = "TRAIL_6";
        public const string CURRENT_CAR = "CURRENT_CAR";
        public const string CURRENT_TRAIL = "CURRENT_TRAIL";
    }
}