using Xunit;
using MiView.Common.Connection.WebSocket;
using System.Net.WebSockets;

namespace MiView.Tests.Common.Connection.WebSocket
{
    public class WebSocketManagerTests
    {
        private class TestWebSocketManager : WebSocketManager
        {
            public TestWebSocketManager() : base() { }

            public new void SetUserInitiatedDisconnect(bool isUserInitiated)
            {
                base.SetUserInitiatedDisconnect(isUserInitiated);
            }

            public new bool IsUserInitiatedDisconnect()
            {
                return base.IsUserInitiatedDisconnect();
            }

            public new ClientWebSocket GetSocketClient()
            {
                return base.GetSocketClient();
            }

            public new WebSocketState? GetSocketState()
            {
                return base.GetSocketState();
            }

            public new void SetDataGridTimeLine(object timeLine)
            {
                base.SetDataGridTimeLine(timeLine);
            }
        }

        [Fact]
        public void SetUserInitiatedDisconnect_true設定_正しく設定される()
        {
            // Arrange
            var manager = new TestWebSocketManager();

            // Act
            manager.SetUserInitiatedDisconnect(true);

            // Assert
            Assert.True(manager.IsUserInitiatedDisconnect());
        }

        [Fact]
        public void SetUserInitiatedDisconnect_false設定_正しく設定される()
        {
            // Arrange
            var manager = new TestWebSocketManager();

            // Act
            manager.SetUserInitiatedDisconnect(false);

            // Assert
            Assert.False(manager.IsUserInitiatedDisconnect());
        }

        [Fact]
        public void GetSocketClient_初期状態_WebSocketオブジェクトを返す()
        {
            // Arrange
            var manager = new TestWebSocketManager();

            // Act
            var socket = manager.GetSocketClient();

            // Assert
            Assert.NotNull(socket);
            Assert.IsType<ClientWebSocket>(socket);
        }

        [Fact]
        public void GetSocketState_初期状態_None状態を返す()
        {
            // Arrange
            var manager = new TestWebSocketManager();

            // Act
            var state = manager.GetSocketState();

            // Assert
            Assert.Equal(WebSocketState.None, state);
        }

        [Fact]
        public void SetDataGridTimeLine_オブジェクト追加_正常に追加される()
        {
            // Arrange
            var manager = new TestWebSocketManager();
            var testObject = new object();

            // Act & Assert
            var exception = Record.Exception(() => manager.SetDataGridTimeLine(testObject));
            Assert.Null(exception);
        }

        [Fact]
        public void IsUserInitiatedDisconnect_初期状態_falseを返す()
        {
            // Arrange
            var manager = new TestWebSocketManager();

            // Act
            var result = manager.IsUserInitiatedDisconnect();

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void Dispose_正常に実行される()
        {
            // Arrange
            var manager = new TestWebSocketManager();

            // Act & Assert
            var exception = Record.Exception(() => manager.Dispose());
            Assert.Null(exception);
        }
    }
}