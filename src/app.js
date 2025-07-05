// エントリーポイント
const express = require('express');
const app = express();

// ミドルウェア設定例
app.use(express.json());
app.use(express.static('public'));

// ビューエンジン設定
app.set('views', path.join(__dirname, 'views'));
app.set('view engine', 'ejs');

// ルート例
app.get('/', (req, res) => {
  res.send('Hello, Express!');
});

// サーバー起動
const PORT = process.env.PORT || 3000;
app.listen(PORT, () => {
  console.log(`Server is running on port ${PORT}`);
});
