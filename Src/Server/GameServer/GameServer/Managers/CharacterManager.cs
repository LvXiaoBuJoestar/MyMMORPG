using Common;
using GameServer.Entities;
using SkillBridge.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Managers
{
    internal class CharacterManager : Singleton<CharacterManager>
    {
        public Dictionary<int, Character> characters = new Dictionary<int, Character>();

        public CharacterManager() { }

        public void Dispose()
        {

        }

        public void Init()
        {

        }

        public void Clear()
        {
            characters.Clear();
        }

        public Character AddCharacter(TCharacter character)
        {
            Character cha = new Character(CharacterType.Player, character);
            characters[cha.Id] = cha;
            return cha;
        }

        public void RemoveCharacter(int characterId)
        {
            characters.Remove(characterId);
        }
    }
}
