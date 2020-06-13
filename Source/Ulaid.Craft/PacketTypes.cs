namespace Ulaid.Craft
{
    public class PacketTypes
    {
        /// <summary>
        /// IDs for packets sent from the client to the server
        /// </summary>
        public enum Serverbound
        {
            // Handshake
            Handshake,
            LegacyServerListPing = 0xFE,

            // Server list ping
            Request = 0x00,
            Ping,

            // Login
            LoginStart = 0x00,

            // Play
            TeleportConfirm = 0x00,
            TabComplete,
            ChatMessage,
            ClientStatus,
            ClientSettings,
            ConfirmTransaction,
            EnchantItem,
            ClickWindow,
            CloseWindow,
            PluginMessage,
            UseEntity,
            KeepAlive,
            Player,
            PlayerPosition,
            PlayerPositionAndLook,
            PlayerLook,
            VehicleMove,
            SteerBoat,
            CraftRecipeRequest,
            PlayerAbilities,
            PlayerDigging,
            EntityAction,
            SteerVehicle,
            CraftingBookData,
            ResourcePackStatus,
            AdvancementTab,
            HeldItemChange,
            CreativeInventoryAction,
            UpdateSign,
            Animation,
            Spectate,
            PlayerBlockPlacement,
            UseItem
        }
        
        /// <summary>
        /// IDs for packets sent from the server to the client
        /// </summary>
        public enum Clientbound
        {
            // Server list ping
            Response,
            Pong,

            // Play
            SpawnObject = 0x00,
            SpawnExperienceOrb,
            SpawnGlobalEntity,
            SpawnMob,
            SpawnPainting,
            SpawnPlayer,
            Animation,
            Statictics,
            BlockBreakAnimation,
            UpdateBlockEntity,
            BlockAction,
            BlockChange,
            BossBar,
            ServerDifficulty,
            TabComplete,
            ChatMessage,
            MultiBlockChange,
            ConfirmTransaction,
            CloseWindow,
            OpenWindow,
            WindowItems,
            WindowProperty,
            SetSlot,
            SetCooldown,
            PluginMessage,
            NamedSoundEffect,
            Disconnect,
            EntityStatus,
            Explosion,
            UnloadChunk,
            ChangeGameState,
            KeepAlive,
            ChunkData,
            Effect,
            Particle,
            JoinGame,
            Map,
            Entity,
            EntityRelativeMove,
            EntityLookAndRelativeMove,
            EntityLook,
            VehicleMove,
            OpenSignEditor,
            CraftRecipeResponse,
            PlayerAbilities,
            CombatEvent,
            PlayerListItem,
            PlayerPositionAndLook,
            UseBed,
            UnlockRecipes,
            DestroyEntities,
            RemoveEntityEffect,
            ResourcePackSend,
            Respawn,
            EntityHeadLook,
            SelectAdvancementTab,
            WorldBorder,
            Camera,
            HeldItemChange,
            DisplayScoreboard,
            EntityMetadata,
            AttachEntity,
            EntityVelocity,
            EntityEquipment,
            SetExperience,
            UpdateHealth,
            ScoreboardObjective,
            SetPassengers,
            Teams,
            UpdateScore,
            SpawnPosition,
            TimeUpdate,
            Title,
            SoundEffect,
            PlayerListHeaderAndFooter,
            CollectItem,
            EntityTeleport,
            Advancements,
            EntityProperties,
            EntityEffect
        }
    }
}
