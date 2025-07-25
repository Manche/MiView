using Xunit;
using Moq;
using MiView.Common.Connection;
using MiView.Common.Connection.WebSocket.Misskey.v2025;
using System.Net.WebSockets;

namespace MiView.Tests.Common.Connection
{
    public class WebSocketConnectionManagerTests
    {
        [Fact]
        public void コンストラクタ_正常に初期化される()
        {
            // Arrange & Act
            var manager = new WebSocketConnectionManager();

            // Assert  
            Assert.NotNull(manager);
        }

        [Fact]
        public void DebugConnectionStatus_接続がない場合_エラーが発生しない()
        {
            // Arrange
            var manager = new WebSocketConnectionManager();

            // Act & Assert
            var exception = Record.Exception(() => manager.DebugConnectionStatus());
            Assert.Null(exception);
        }

        [Fact]
        public async Task DisconnectAll_ユーザー操作による切断_正常に実行される()
        {
            // Arrange
            var manager = new WebSocketConnectionManager();

            // Act & Assert
            var exception = await Record.ExceptionAsync(async () => 
                await manager.DisconnectAll(isUserInitiated: true));
            Assert.Null(exception);
        }

        [Fact]
        public async Task DisconnectAll_自動切断_正常に実行される()
        {
            // Arrange
            var manager = new WebSocketConnectionManager();

            // Act & Assert
            var exception = await Record.ExceptionAsync(async () => 
                await manager.DisconnectAll(isUserInitiated: false));
            Assert.Null(exception);
        }

        [Fact]
        public void GetUnifiedTimelineConnections_初期状態_空のリストを返す()
        {
            // Arrange
            var manager = new WebSocketConnectionManager();

            // Act
            var connections = manager.GetUnifiedTimelineConnections();

            // Assert
            Assert.NotNull(connections);
            Assert.Empty(connections);
        }

        [Fact]
        public void GetConnection_存在しないインスタンス_nullを返す()
        {
            // Arrange
            var manager = new WebSocketConnectionManager();

            // Act
            var connection = manager.GetConnection("nonexistent", "ローカルTL");

            // Assert
            Assert.Null(connection);
        }

        [Fact]
        public async Task DisconnectInstance_存在しないインスタンス_エラーが発生しない()
        {
            // Arrange
            var manager = new WebSocketConnectionManager();

            // Act & Assert
            var exception = await Record.ExceptionAsync(async () => 
                await manager.DisconnectInstance("nonexistent", isUserInitiated: true));
            Assert.Null(exception);
        }
    }
}