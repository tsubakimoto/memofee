// エントリーポイント
const express = require('express');
const app = express();

// ミドルウェア設定例
app.use(express.json());

// ルート例
app.get('/', (req, res) => {
  res.send('Hello, Express!');
});

// サーバー起動
const PORT = process.env.PORT || 3000;
app.listen(PORT, () => {
  console.log(`Server is running on port ${PORT}`);
});
