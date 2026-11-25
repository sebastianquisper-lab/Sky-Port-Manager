const $ = (id) => document.getElementById(id);
const state = { session: null, demo: false, flights: [], occupancy: null, view: 'dashboard', generation: 0, expiryTimer: null };
const escape = (value) => String(value ?? '').replace(/[&<>"']/g, (c) => ({ '&': '&amp;', '<': '&lt;', '>': '&gt;', '"': '&quot;', "'": '&#39;' })[c]);
const number = new Intl.NumberFormat('es-PE', { maximumFractionDigits: 1 });
const localDay = (value) => {
  const date = new Date(value);
  return Number.isNaN(date.getTime()) ? '' : `${date.getFullYear()}-${String(date.getMonth() + 1).padStart(2, '0')}-${String(date.getDate()).padStart(2, '0')}`;
};
const dateTime = (value, options) => value && !Number.isNaN(new Date(value).getTime())
  ? new Intl.DateTimeFormat('es-PE', options).format(new Date(value)) : '—';
const time = (value) => dateTime(value, { hour: '2-digit', minute: '2-digit' });
const day = (value) => dateTime(value, { day: '2-digit', month: 'short' });
function statusInfo(status) {
  const raw = String(status ?? '').trim();
  const key = raw.toUpperCase().replaceAll(' ', '_');
  const known = {
    SCHEDULED: ['Programado', ''], PROGRAMADO: ['Programado', ''],
    BOARDING: ['Embarcando', 'success'], EMBARCANDO: ['Embarcando', 'success'],
    DEPARTED: ['Despegó', 'success'], IN_FLIGHT: ['En vuelo', 'success'],
    LANDED: ['Aterrizó', 'success'], ARRIVED: ['Llegó', 'success'],
    DELAYED: ['Retrasado', 'warning'], RETRASADO: ['Retrasado', 'warning'],
    CANCELLED: ['Cancelado', 'danger'], CANCELED: ['Cancelado', 'danger'], CANCELADO: ['Cancelado', 'danger'],
  };
  return known[key] ?? [raw || 'Sin estado', ''];
}
function badge(status) {
  const [label, tone] = statusInfo(status);
  return `<span class="badge ${tone}">${escape(label)}</span>`;
}
async function api(path, options = {}) {
  const headers = { Accept: 'application/json', ...options.headers };
  if (state.session) headers.Authorization = `Bearer ${state.session.accessToken}`;
  let response;
  try {
    response = await fetch(`/api/${path}`, { ...options, headers, signal: AbortSignal.timeout(20000) });
  } catch {
    throw new Error('No se pudo conectar con la API. Comprueba que el servidor esté disponible e intenta actualizar.');
  }
  if (response.status === 401 && path !== 'auth/login') {
    logout('Tu sesión venció. Inicia sesión nuevamente.');
    throw new Error('Sesión vencida.');
  }
  if (!response.ok) {
    // La API puede incluir detalles internos de SQL; no se muestran directamente.
    const messages = { 400: 'Revisa los datos ingresados.', 401: 'Usuario o contraseña incorrectos.', 403: 'Tu cuenta no tiene permiso para consultar estos datos.', 404: 'No se encontró el recurso solicitado.' };
    throw new Error(messages[response.status] ?? `El servidor no pudo completar la consulta (HTTP ${response.status}). Inténtalo nuevamente.`);
  }
  return response.json();
}
function demoData() {
  const date = localDay(new Date());
  const rows = [
    ['LA 2041', 'LATAM Airlines', 'LIM', 'CUZ', '07:20', '08:45', 'ARRIVED', 'A04', 144, 160],
    ['H2 5102', 'SKY Airline', 'LIM', 'AQP', '09:15', '10:50', 'DEPARTED', 'A08', 126, 150],
    ['JA 7031', 'JetSMART', 'LIM', 'PIU', '11:40', '13:10', 'BOARDING', 'B02', 162, 180],
    ['LA 2380', 'LATAM Airlines', 'LIM', 'BOG', '12:10', '15:25', 'DELAYED', 'B06', 152, 180],
    ['CM 492', 'Copa Airlines', 'LIM', 'PTY', '14:30', '18:05', 'SCHEDULED', 'B10', 120, 160],
    ['H2 5140', 'SKY Airline', 'LIM', 'TRU', '16:45', '18:00', 'SCHEDULED', 'A03', 108, 150],
    ['LA 2468', 'LATAM Airlines', 'LIM', 'SCL', '18:20', '21:50', 'SCHEDULED', 'B04', 150, 180],
    ['JA 7050', 'JetSMART', 'LIM', 'CIX', '20:00', '21:25', 'CANCELLED', null, 0, 180],
  ];
  return {
    flights: rows.map((r, i) => ({ flightId: i + 1, flightCode: r[0], airlineName: r[1], originIata: r[2], destinationIata: r[3], scheduledDepartureUtc: new Date(`${date}T${r[4]}:00`).toISOString(), scheduledArrivalUtc: new Date(`${date}T${r[5]}:00`).toISOString(), flightStatus: r[6], departureGate: r[7], arrivalGate: null })),
    occupancy: rows.map((r, i) => ({ flightId: i + 1, ticketsSold: r[8], seatCapacity: r[9], registrationCode: `DEMO-${i + 1}` })),
  };
}
function enter(session, demo = false) {
  state.session = session;
  state.demo = demo;
  $('login-view').hidden = true;
  $('app-view').hidden = false;
  $('demo-notice').hidden = !demo;
  $('connection-label').textContent = demo ? 'Vista de demostración' : 'Conectado a la API';
  $('user-name').textContent = session?.username ?? 'Visitante';
  $('user-role').textContent = session?.role ?? 'Demostración';
  $('user-avatar').textContent = (session?.username ?? 'SP').slice(0, 2).toUpperCase();
  $('date-filter').value = localDay(new Date());
  $('search').value = '';
  $('status-filter').value = '';
  setView('dashboard');
  if (session) {
    state.expiryTimer = setTimeout(() => logout('Tu sesión venció. Inicia sesión nuevamente.'), Math.max(0, new Date(session.expiresAtUtc).getTime() - Date.now()));
  }
  load();
}
function logout(message = '') {
  clearTimeout(state.expiryTimer);
  state.generation++;
  state.session = null;
  state.demo = false;
  state.flights = [];
  state.occupancy = null;
  $('app-view').hidden = true;
  $('login-view').hidden = false;
  $('flight-dialog').close();
  $('password').value = '';
  $('login-error').textContent = message;
  $('flight-rows').replaceChildren();
  $('stats').replaceChildren();
  $('detail-content').replaceChildren();
  $('username').focus();
}
function setView(view) {
  state.view = view;
  const dashboard = view === 'dashboard';
  $('dashboard-section').hidden = !dashboard;
  $('page-title').textContent = dashboard ? 'Resumen de operaciones' : 'Gestión de vuelos';
  $('page-subtitle').textContent = dashboard ? 'Tu tráfico aéreo, en una sola vista.' : 'Consulta rutas, horarios y estados de tu operación.';
  $('breadcrumb-current').textContent = dashboard ? 'Resumen' : 'Vuelos';
  document.querySelectorAll('[data-view]').forEach((button) => {
    button.classList.toggle('active', button.dataset.view === view);
    if (button.dataset.view === view) button.setAttribute('aria-current', 'page');
    else button.removeAttribute('aria-current');
  });
}
async function load() {
  const generation = ++state.generation;
  $('loading').hidden = false;
  $('refresh').disabled = true;
  $('app-error').hidden = true;
  try {
    let flights, occupancy;
    if (state.demo) ({ flights, occupancy } = demoData());
    else {
      const results = await Promise.allSettled([api('flight-board'), api('flight-occupancy')]);
      if (generation !== state.generation) return;
      if (results[0].status === 'rejected') throw results[0].reason;
      flights = results[0].value;
      occupancy = results[1].status === 'fulfilled' ? results[1].value : null;
      if (!Array.isArray(flights) || (occupancy !== null && !Array.isArray(occupancy))) throw new Error('La respuesta del servidor no tiene el formato esperado.');
      if (occupancy === null) showError('Los vuelos están disponibles, pero no se pudo consultar la ocupación. Puedes intentar actualizar.');
    }
    if (generation !== state.generation) return;
    state.flights = flights.slice().sort((a, b) => new Date(a.scheduledDepartureUtc) - new Date(b.scheduledDepartureUtc));
    state.occupancy = occupancy;
    const selected = $('status-filter').value;
    const statuses = [...new Set(flights.map((f) => f.flightStatus).filter(Boolean))].sort();
    $('status-filter').innerHTML = '<option value="">Todos los estados</option>' + statuses.map((s) => `<option value="${escape(s)}">${escape(statusInfo(s)[0])}</option>`).join('');
    if (statuses.includes(selected)) $('status-filter').value = selected;
    renderDashboard();
    renderFlights();
    $('updated-at').textContent = `Actualizado a las ${time(new Date())}`;
  } catch (error) {
    if (generation === state.generation) showError(`${error.message} Los datos visibles corresponden a la última consulta exitosa, si existe.`);
  } finally {
    if (generation === state.generation) {
      $('loading').hidden = true;
      $('refresh').disabled = false;
    }
  }
}
function showError(message) {
  $('app-error').textContent = message;
  $('app-error').hidden = false;
}
function renderDashboard() {
  const today = state.flights.filter((f) => localDay(f.scheduledDepartureUtc) === localDay(new Date()));
  const delayed = today.filter((f) => statusInfo(f.flightStatus)[1] === 'warning').length;
  const departed = today.filter((f) => f.actualDepartureUtc || ['DEPARTED', 'IN_FLIGHT', 'LANDED', 'ARRIVED'].includes(String(f.flightStatus).toUpperCase())).length;
  const ids = new Set(today.map((f) => f.flightId));
  const seats = state.occupancy?.filter((o) => ids.has(o.flightId) && o.seatCapacity > 0 && o.ticketsSold != null) ?? [];
  const capacity = seats.reduce((sum, o) => sum + Number(o.seatCapacity), 0);
  const sold = seats.reduce((sum, o) => sum + Number(o.ticketsSold), 0);
  const cards = [
    ['Vuelos de hoy', today.length, 'Salidas programadas para hoy', '✈'],
    ['Vuelos con salida', departed, 'Despegados, en vuelo o llegados', '↗'],
    ['Vuelos retrasados', delayed, 'Según el estado registrado', '◷'],
    ['Ocupación', capacity ? `${number.format(sold / capacity * 100)}%` : '—', capacity ? `${seats.length} vuelos con datos de capacidad` : 'Sin datos de capacidad para hoy', '▥'],
  ];
  $('stats').innerHTML = cards.map(([title, value, note, icon]) => `<article class="stat-card"><div class="stat-top"><span>${title}</span><span class="stat-icon" aria-hidden="true">${icon}</span></div><div class="stat-value">${value}</div><div class="stat-note">${note}</div></article>`).join('');
  const boarding = today.filter((f) => String(f.flightStatus).toUpperCase() === 'BOARDING').length;
  const scheduled = today.filter((f) => ['SCHEDULED', 'PROGRAMADO'].includes(String(f.flightStatus).toUpperCase())).length;
  $('status-summary').innerHTML = [['Programados', scheduled, ''], ['Embarcando', boarding, 'success'], ['Retrasados', delayed, 'warning']].map(([label, count, tone]) => `<div class="status-row"><span>${label}</span><div class="progress-track"><div class="progress-fill ${tone}" style="width:${today.length ? count / today.length * 100 : 0}%"></div></div><strong>${count}</strong></div>`).join('');
  $('traffic-summary').textContent = today.length ? `${today.length} vuelos en el tablero de hoy. Consulta cada ruta y acompaña la operación.` : 'No hay salidas registradas para hoy. Explora otras fechas en el tablero.';
}
function renderFlights() {
  const query = $('search').value.trim().toLocaleLowerCase('es');
  const date = $('date-filter').value;
  const status = $('status-filter').value;
  const rows = state.flights.filter((f) => (!date || localDay(f.scheduledDepartureUtc) === date)
    && (!status || f.flightStatus === status)
    && [f.flightCode, f.airlineName, f.originIata, f.destinationIata].join(' ').toLocaleLowerCase('es').includes(query));
  $('result-count').textContent = `${rows.length} ${rows.length === 1 ? 'vuelo' : 'vuelos'}`;
  $('empty-state').hidden = rows.length > 0;
  $('flight-rows').innerHTML = rows.map((f) => `<tr><td><strong>${escape(f.flightCode || 'Sin código')}</strong><small>${escape(f.airlineName || 'Sin aerolínea')}</small></td><td><div class="route">${escape(f.originIata || '—')}<span aria-label="a">—</span>${escape(f.destinationIata || '—')}</div></td><td><strong>${time(f.scheduledDepartureUtc)}</strong><small>${day(f.scheduledDepartureUtc)}</small></td><td><strong>${time(f.scheduledArrivalUtc)}</strong><small>${day(f.scheduledArrivalUtc)}</small></td><td>${escape(f.departureGate || 'Por asignar')}</td><td>${badge(f.flightStatus)}</td><td><button class="text-button detail-button" data-id="${escape(f.flightId)}" aria-label="Ver detalle de ${escape(f.flightCode)}">Detalle</button></td></tr>`).join('');
}
function showDetail(id) {
  const flight = state.flights.find((f) => String(f.flightId) === id);
  if (!flight) return;
  const occupancy = state.occupancy?.find((o) => o.flightId === flight.flightId);
  $('detail-title').textContent = flight.flightCode || 'Vuelo sin código';
  const fields = [
    ['Aerolínea', escape(flight.airlineName || '—')], ['Estado', badge(flight.flightStatus)],
    ['Salida programada', escape(`${day(flight.scheduledDepartureUtc)} · ${time(flight.scheduledDepartureUtc)}`)],
    ['Llegada programada', escape(`${day(flight.scheduledArrivalUtc)} · ${time(flight.scheduledArrivalUtc)}`)],
    ['Salida real', escape(flight.actualDepartureUtc ? `${day(flight.actualDepartureUtc)} · ${time(flight.actualDepartureUtc)}` : 'No registrada')],
    ['Llegada real', escape(flight.actualArrivalUtc ? `${day(flight.actualArrivalUtc)} · ${time(flight.actualArrivalUtc)}` : 'No registrada')],
    ['Puerta de salida', escape(flight.departureGate || 'Por asignar')], ['Puerta de llegada', escape(flight.arrivalGate || 'Por asignar')],
    ['Matrícula', escape(occupancy?.registrationCode || 'Sin datos')],
    ['Boletos / Capacidad', occupancy?.ticketsSold != null && occupancy?.seatCapacity != null ? `${escape(occupancy.ticketsSold)} / ${escape(occupancy.seatCapacity)}` : 'Sin datos'],
  ];
  $('detail-content').innerHTML = `<div class="detail-route"><strong>${escape(flight.originIata || '—')}</strong><span aria-hidden="true">✈</span><strong>${escape(flight.destinationIata || '—')}</strong></div><dl class="detail-list">${fields.map(([label, value]) => `<div><dt>${label}</dt><dd>${value}</dd></div>`).join('')}</dl><p class="detail-note">Horarios mostrados en ${escape(Intl.DateTimeFormat().resolvedOptions().timeZone)}.</p>`;
  $('flight-dialog').showModal();
}
$('login-form').addEventListener('submit', async (event) => {
  event.preventDefault();
  $('login-error').textContent = '';
  $('login-submit').disabled = true;
  $('demo-button').disabled = true;
  $('login-submit').textContent = 'Ingresando…';
  try {
    const session = await api('auth/login', { method: 'POST', headers: { 'Content-Type': 'application/json' }, body: JSON.stringify({ username: $('username').value.trim(), password: $('password').value }) });
    if (!session.accessToken || !(new Date(session.expiresAtUtc).getTime() > Date.now())) throw new Error('El servidor devolvió una sesión inválida.');
    $('password').value = '';
    enter(session);
  } catch (error) { $('login-error').textContent = error.message; }
  finally {
    $('login-submit').disabled = false;
    $('demo-button').disabled = false;
    $('login-submit').textContent = 'Iniciar sesión';
  }
});
$('demo-button').addEventListener('click', () => enter(null, true));
$('logout').addEventListener('click', () => logout());
$('refresh').addEventListener('click', load);
$('toggle-password').addEventListener('click', () => {
  const visible = $('password').type === 'password';
  $('password').type = visible ? 'text' : 'password';
  $('toggle-password').textContent = visible ? 'Ocultar' : 'Ver';
  $('toggle-password').setAttribute('aria-label', visible ? 'Ocultar contraseña' : 'Mostrar contraseña');
  $('toggle-password').setAttribute('aria-pressed', String(visible));
});
document.querySelectorAll('[data-view]').forEach((button) => button.addEventListener('click', () => setView(button.dataset.view)));
$('view-all').addEventListener('click', () => { setView('flights'); $('search').focus(); });
['search', 'date-filter', 'status-filter'].forEach((id) => $(id).addEventListener('input', renderFlights));
$('clear-filters').addEventListener('click', () => { ['search', 'date-filter', 'status-filter'].forEach((id) => $(id).value = ''); renderFlights(); });
$('flight-rows').addEventListener('click', (event) => { const button = event.target.closest('.detail-button'); if (button) showDetail(button.dataset.id); });
$('close-dialog').addEventListener('click', () => $('flight-dialog').close());
$('timezone').textContent = Intl.DateTimeFormat().resolvedOptions().timeZone;
