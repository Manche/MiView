# MiView 開発者ドキュメント

## プロジェクト概要

MiViewは、Misskeyインスタンスのタイムラインを表示するクロスプラットフォームデスクトップアプリケーションです。
Avalonia UIフレームワークを使用して、Windows、Linux、macOSで動作します。

## 技術スタック

- **.NET 9.0**: メインフレームワーク
- **Avalonia 11.0.7**: UIフレームワーク
- **Material Icons**: アイコンフォント
- **WebSocket**: リアルタイム通信
- **System.Text.Json**: JSON処理

## 開発環境セットアップ

### 必要な環境

- .NET 9.0 SDK
- 任意のIDE（Visual Studio、VS Code、Rider等）

### プロジェクト構造

```
MiView/
├── MiView/
│   ├── Common/                    # 共通ライブラリ
│   │   ├── AnalyzeData/          # データ解析
│   │   ├── Connection/           # WebSocket接続
│   │   ├── Fonts/                # フォント管理
│   │   └── TimeLine/             # タイムライン処理
│   ├── ScreenForms/              # UI画面
│   │   └── PlainForm/            # メイン画面
│   ├── Resources/                # リソースファイル
│   │   ├── Fonts/                # フォントファイル
│   │   └── Styles/               # スタイル定義
│   └── MainWindow.axaml.cs       # メインウィンドウ
```

## ビルド方法

### Linux（開発環境）

```bash
# 依存関係の復元
dotnet restore

# ビルド
dotnet build

# 実行
dotnet run
```

### Windows

```bash
# プロジェクトファイルのRuntimeIdentifierを変更
# <RuntimeIdentifier>win-x64</RuntimeIdentifier>

# ビルド
dotnet build

# 実行
dotnet run
```

### macOS

```bash
# プロジェクトファイルのRuntimeIdentifierを変更
# <RuntimeIdentifier>osx-x64</RuntimeIdentifier>

# ビルド
dotnet build

# 実行
dotnet run
```

## デバッグ機能

### Debugメニュー

Debugビルド時のみ表示される「デバッグ」メニューから以下の機能が利用できます：

- **ダミーデータ生成**: 各状態（公開範囲、連合状態、リノート）の組み合わせを一瞬で生成

### 生成されるダミーデータ

1. **公開範囲**
   - Public（公開）: 地球アイコン（緑色）
   - Home（ホーム）: 家アイコン（オレンジ色）
   - Followers（フォロワー）: 鍵アイコン（赤色）
   - Direct（ダイレクト）: メールアイコン（紫色）

2. **連合状態**
   - Local（ローカル）: 宇宙船アイコン（赤色）
   - Remote（リモート）: 宇宙船アイコン（緑色）

3. **リノート**
   - あり: リピートマーク（緑色）
   - なし: 表示なし

## コード構造

### メインウィンドウ（MainWindow.axaml.cs）

- **定数管理**: `UIColors`クラスで色の定数を管理
- **タイムライン表示**: `TimelineDisplayElements`で列の順序を定義
- **データ処理**: WebSocketからのデータを受信・処理
- **UI更新**: Dispatcherを使用したスレッドセーフなUI更新

### フォント管理（FontLoader.cs）

- Material Iconsフォントの読み込み
- Avaloniaリソースからのフォント取得

### タイムライン処理（TimeLineCreator.cs）

- タイムラインデータの構造定義
- 表示要素の列挙型定義

## 設定ファイル

### settings.json

```json
{
  "Instances": ["misskey.io", "mi.ruruke.moe"],
  "InstanceTokens": {
    "misskey.io": "your-api-token-here"
  }
}
```

## トラブルシューティング

### よくある問題

1. **ビルドエラー**: `arch-x64`エラー
   - 解決策: キャッシュをクリア（`rm -rf obj bin`）

2. **フォントが表示されない**
   - 確認: Material Iconsフォントファイルが正しく配置されているか
   - 確認: Avaloniaリソースとして正しく登録されているか

3. **WebSocket接続エラー**
   - 確認: インスタンスURLが正しいか
   - 確認: APIトークンが有効か

### ログ出力

Debugビルド時は以下の情報がコンソールに出力されます：

- WebSocket接続状態
- データ受信状況
- UI更新状況

## 開発ガイドライン

### コードスタイル

- C#の標準的なコーディング規約に従う
- 日本語コメントを使用
- 定数はクラスとしてまとめる
- メソッドは単一責任の原則に従う

### リファクタリング

- 既存の動作を壊さない範囲で実施
- ビルド成功を確認してから次の変更を行う
- 色の定数化やメソッド分割を優先

### テスト

- Debugメニューのダミーデータ生成で動作確認
- 複数のインスタンスでの接続テスト
- 異なる公開範囲での表示確認 