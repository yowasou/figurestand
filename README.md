# figurestand
figurestand for unity

Android上でMMDモデルを眺めるためのアプリのテンプレートです。主にタッチ操作のところをがんばりました。

コードはどこを他で使ってもかまいません。

実際に動かすにはUnityとAndroid SDKとMMDモデルとモーションとMMD4Mecanimが必要です。超めんどい。

Unityにもgemsetみたいな仕組希望。


- UnityでAssets/1.unityを開く
- Stereoarts様に超感謝しながらMMD4Mecanimをダウンロードし、Unityへインポートする
- http://stereoarts.jp/
- MMDモデルをフォルダごとAssets/Resources/Modelへ置く
- VMDファイルをAssets/Motionへ置く
- MMDモデルをMMD4MecanimでFBXへ変換。RigはLegacyにする
- FBXになったMMDモデルをシーンの0,0,0へ配置。TagにMainFigureを設定
- 無事実行されることを祈りつつ実行する
- さらに祈りつつAndroid用に実行する


ビルド手順の詳細はブログでも紹介しています。

http://appharbormemo.blogspot.jp/2015/03/mmdandroid.html
