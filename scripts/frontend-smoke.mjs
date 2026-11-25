// Browser smoke checks with Node.js 22+ and Microsoft Edge. No npm dependencies.
import { createServer } from 'node:http';
import { readFile, mkdtemp, rm } from 'node:fs/promises';
import { join, extname, dirname, basename, resolve } from 'node:path';
import { tmpdir } from 'node:os';
import { spawn } from 'node:child_process';
import assert from 'node:assert/strict';
const root = new URL('../Sky.API/wwwroot/', import.meta.url);
let failBoard = false;
let badLogin = false;
let authenticatedRequests = 0;
const today = new Date();
const flights = [{ flightId: 99, flightCode: 'TEST 99', airlineName: '<img src=x onerror=alert(1)>', originIata: 'LIM', destinationIata: 'CUZ', scheduledDepartureUtc: today.toISOString(), scheduledArrivalUtc: today.toISOString(), flightStatus: 'BOARDING', departureGate: 'A01' }];
const server = createServer(async (req, res) => {
  try {
    const path = new URL(req.url, 'http://localhost').pathname;
    if (path.startsWith('/api/')) {
      res.setHeader('Content-Type', 'application/json');
      if (path === '/api/auth/login') {
        res.statusCode = badLogin ? 401 : 200;
        res.end(JSON.stringify(badLogin ? {} : { accessToken: 'smoke-token', username: 'operador', role: 'Operations', expiresAtUtc: new Date(Date.now() + 60000).toISOString() }));
        return;
      }
      if (req.headers.authorization !== 'Bearer smoke-token') { res.writeHead(401).end('{}'); return; }
      authenticatedRequests++;
      if (failBoard && path === '/api/flight-board') { res.writeHead(503).end('{}'); return; }
      res.end(JSON.stringify(path === '/api/flight-board' ? flights : [{ flightId: 99, seatCapacity: 100, ticketsSold: 80 }]));
      return;
    }
    const file = path === '/' ? 'index.html' : path.slice(1);
    if (!['index.html', 'styles.css', 'app.js'].includes(file)) { res.writeHead(404).end(); return; }
    res.setHeader('Content-Type', { '.html': 'text/html', '.css': 'text/css', '.js': 'text/javascript' }[extname(file)]);
    res.end(await readFile(new URL(file, root)));
  } catch { res.writeHead(500).end(); }
});
await new Promise((resolve) => server.listen(0, '127.0.0.1', resolve));
const url = `http://127.0.0.1:${server.address().port}`;
const profile = await mkdtemp(join(tmpdir(), 'skyport-browser-'));
const edge = spawn(process.env.EDGE_PATH ?? 'C:/Program Files (x86)/Microsoft/Edge/Application/msedge.exe', ['--headless=new', '--disable-gpu', '--no-first-run', '--no-default-browser-check', '--remote-debugging-port=0', `--user-data-dir=${profile}`, 'about:blank'], { windowsHide: true, stdio: 'ignore' });
let socket;
const delay = (ms) => new Promise((resolve) => setTimeout(resolve, ms));
try {
  let port;
  for (let i = 0; i < 100; i++) {
    try { port = Number((await readFile(join(profile, 'DevToolsActivePort'), 'utf8')).split('\n')[0]); break; } catch { await delay(100); }
  }
  assert.ok(port, 'Edge debugging port available');
  const targets = await (await fetch(`http://127.0.0.1:${port}/json/list`)).json();
  socket = new WebSocket(targets.find((target) => target.type === 'page').webSocketDebuggerUrl);
  await new Promise((resolve, reject) => { socket.addEventListener('open', resolve); socket.addEventListener('error', reject); });
  let id = 0;
  const pending = new Map();
  socket.addEventListener('message', (event) => { const result = JSON.parse(event.data); if (result.id) { pending.get(result.id)?.(result); pending.delete(result.id); } });
  const cdp = (method, params = {}) => new Promise((resolve, reject) => {
    const key = ++id;
    const timeout = setTimeout(() => { pending.delete(key); reject(new Error(`Timed out: ${method}`)); }, 10000);
    pending.set(key, (response) => { clearTimeout(timeout); response.error ? reject(new Error(response.error.message)) : resolve(response.result); });
    socket.send(JSON.stringify({ id: key, method, params }));
  });
  const evaluate = async (expression) => {
    const result = await cdp('Runtime.evaluate', { expression, returnByValue: true, awaitPromise: true });
    if (result.exceptionDetails) throw new Error(JSON.stringify(result.exceptionDetails));
    return result.result.value;
  };
  const until = async (expression) => {
    for (let i = 0; i < 100; i++) { if (await evaluate(expression)) return; await delay(50); }
    throw new Error(`Condition not met: ${expression}`);
  };
  await cdp('Emulation.setDeviceMetricsOverride', { width: 1440, height: 1100, deviceScaleFactor: 1, mobile: false });
  await cdp('Page.navigate', { url });
  await until('!!document.getElementById("demo-button")');
  await evaluate('document.getElementById("demo-button").click()');
  await until('document.querySelectorAll("#flight-rows tr").length === 8');
  assert.equal(await evaluate('document.getElementById("demo-notice").hidden'), false);
  await evaluate('document.getElementById("search").value="BOG"; document.getElementById("search").dispatchEvent(new Event("input"))');
  assert.equal(await evaluate('document.querySelectorAll("#flight-rows tr").length'), 1);
  await evaluate('document.querySelector(".detail-button").click()');
  assert.equal(await evaluate('document.getElementById("flight-dialog").open'), true);
  await evaluate('document.getElementById("close-dialog").click(); document.getElementById("clear-filters").click()');
  const image = await cdp('Page.captureScreenshot', { format: 'png' });
  const { writeFile } = await import('node:fs/promises');
  const screenshot = join(tmpdir(), 'skyport-desktop.png');
  await writeFile(screenshot, Buffer.from(image.data, 'base64'));
  await cdp('Emulation.setDeviceMetricsOverride', { width: 390, height: 844, deviceScaleFactor: 1, mobile: true });
  await cdp('Page.navigate', { url });
  await until('!!document.getElementById("demo-button") && document.getElementById("app-view").hidden');
  await evaluate('document.getElementById("demo-button").click()');
  await until('document.querySelectorAll("#flight-rows tr").length === 8');
  assert.equal(await evaluate('document.documentElement.scrollWidth <= innerWidth'), true, 'No page overflow on mobile');
  await writeFile(join(tmpdir(), 'skyport-mobile.png'), Buffer.from((await cdp('Page.captureScreenshot', { format: 'png' })).data, 'base64'));
  await evaluate('document.getElementById("logout").click()');
  badLogin = true;
  await evaluate('document.getElementById("username").value="operador"; document.getElementById("password").value="test"; document.getElementById("login-form").requestSubmit()');
  await until('document.getElementById("login-error").textContent.includes("incorrectos")');
  badLogin = false;
  await evaluate('document.getElementById("login-form").requestSubmit()');
  await until('document.querySelectorAll("#flight-rows tr").length === 1');
  assert.ok(authenticatedRequests >= 2, 'JWT sent to both API endpoints');
  assert.equal(await evaluate('document.querySelectorAll("#flight-rows img").length'), 0, 'API text is escaped');
  failBoard = true;
  await evaluate('document.getElementById("refresh").click()');
  await until('!document.getElementById("app-error").hidden');
  assert.equal(await evaluate('document.querySelectorAll("#flight-rows tr").length'), 1, 'Previous data retained on refresh error');
  await evaluate('document.getElementById("logout").click()');
  assert.equal(await evaluate('document.getElementById("app-view").hidden'), true);
  assert.equal(await evaluate('document.querySelectorAll("#flight-rows tr").length'), 0);
  console.log('PASS: demo, search, details, mobile layout, login errors, JWT requests, escaped API data, refresh failure, logout.');
  console.log(`Desktop screenshot: ${screenshot}`);
} finally {
  socket?.close();
  edge.kill();
  server.closeAllConnections();
  server.close();
  // Edge may take a moment to release its temporary profile.
  await delay(500);
  if (dirname(resolve(profile)) === resolve(tmpdir()) && basename(profile).startsWith('skyport-browser-')) {
    await rm(profile, { recursive: true, force: true, maxRetries: 5, retryDelay: 200 }).catch(() => {});
  }
}
