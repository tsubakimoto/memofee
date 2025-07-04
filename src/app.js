// エントリーポイント
const express = require('express');
const path = require('path');
const app = express();

// ミドルウェア設定例
app.use(express.json());

// ビューエンジン設定
app.set('views', path.join(__dirname, 'views'));
app.set('view engine', 'ejs');

// ルート例
app.get('/', (req, res) => {
  res.send('Hello, Express!');
});

// ひらがな行リストページのルート
const hiraganaRouter = require('./routes/hiragana');
app.use('/hiragana', hiraganaRouter);

// サーバー起動
const PORT = process.env.PORT || 3000;
app.listen(PORT, () => {
  console.log(`Server is running on port ${PORT}`);
});
