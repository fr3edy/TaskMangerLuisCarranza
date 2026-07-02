export interface LoginRequest {
  email: string;
  password: string;
}

export interface LoginResponse {
  token: string;
  // Puedes agregar más campos aquí si tu API de .NET devuelve el ID o nombre del usuario
}