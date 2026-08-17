// src/context/AuthContext.tsx
import { createContext, useContext, useState, type ReactNode } from "react";
import { api } from "../services/api";
import type { AuthResponse } from "../types";

interface AuthContextType {
  user: { name: string; email: string } | null;
  login: (email: string, password: string) => Promise<void>;
  register: (name: string, email: string, password: string) => Promise<void>;
  logout: () => void;
}

const AuthContext = createContext<AuthContextType | null>(null);

export function AuthProvider({ children }: { children: ReactNode }) {
  const [user, setUser] = useState<{ name: string; email: string } | null>(() => {
    const stored = localStorage.getItem("user");
    return stored ? JSON.parse(stored) : null;
  });

  function persist(res: AuthResponse) {
    localStorage.setItem("token", res.token);
    localStorage.setItem("user", JSON.stringify({ name: res.name, email: res.email }));
    setUser({ name: res.name, email: res.email });
  }

  async function login(email: string, password: string) {
    const res = await api.post<AuthResponse>("/auth/login", { email, password });
    persist(res);
  }

  async function register(name: string, email: string, password: string) {
    const res = await api.post<AuthResponse>("/auth/register", { name, email, password });
    persist(res);
  }

  function logout() {
    localStorage.removeItem("token");
    localStorage.removeItem("user");
    setUser(null);
  }

  return (
    <AuthContext.Provider value={{ user, login, register, logout }}>
      {children}
    </AuthContext.Provider>
  );
}

export function useAuth() {
  const ctx = useContext(AuthContext);
  if (!ctx) throw new Error("useAuth must be used within AuthProvider");
  return ctx;
}