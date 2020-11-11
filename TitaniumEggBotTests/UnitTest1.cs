using System;
using Xunit;
using TitaniumEggBot;

namespace TitaniumEggBotTests
{
    public class UnitTest1
    {
        // Needs to be revised
        [Fact]
        public async void CheckNumberOfCharacters_LittleGod()
        {
            var charList = await RotmgApiHelper.GetCharacterInfoInListAsync("LittleGod");
            //Assert.Equal(5, charList.Count);
        }
    }
}
