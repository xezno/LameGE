using ECSEngine;

namespace Ulaid.Content.Scripts {
    class TestScript {
        public int Number { get; private set; } = 12;

        public void Run()
        {
            Debug.Log($"abracadabra; Number is {++Number}");
        }
    }
}
