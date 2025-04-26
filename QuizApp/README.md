# Quiz Game Frontend Implementation Guide

## Project Structure
```
quiz-app-frontend/
├── src/
│   ├── components/
│   │   ├── Room/
│   │   │   ├── CreateRoom.tsx
│   │   │   ├── JoinRoom.tsx
│   │   │   ├── RoomList.tsx
│   │   │   └── RoomLobby.tsx
│   │   ├── Game/
│   │   │   ├── QuestionDisplay.tsx
│   │   │   ├── Leaderboard.tsx
│   │   │   └── GameResults.tsx
│   │   └── common/
│   │       ├── Button.tsx
│   │       └── LoadingSpinner.tsx
│   ├── services/
│   │   ├── api.ts
│   │   ├── signalR.ts
│   │   └── types.ts
│   ├── hooks/
│   │   ├── useRoom.ts
│   │   └── useGame.ts
│   └── pages/
│       ├── Home.tsx
│       ├── Room.tsx
│       └── Game.tsx
```

## Installation
```bash
npm install @microsoft/signalr axios @chakra-ui/react @emotion/react @emotion/styled framer-motion
```

## SignalR Configuration (src/services/signalR.ts)
```typescript
import * as signalR from '@microsoft/signalr';

const connection = new signalR.HubConnectionBuilder()
    .withUrl('/gamehub')
    .withAutomaticReconnect()
    .build();

export const startConnection = async () => {
    try {
        await connection.start();
        console.log('SignalR Connected');
    } catch (err) {
        console.error('Error while establishing connection:', err);
    }
};

export const joinRoom = async (roomCode: string, userId: number) => {
    try {
        await connection.invoke('JoinRoom', roomCode, userId);
    } catch (err) {
        console.error('Error joining room:', err);
    }
};

export const leaveRoom = async (roomCode: string, userId: number) => {
    try {
        await connection.invoke('LeaveRoom', roomCode, userId);
    } catch (err) {
        console.error('Error leaving room:', err);
    }
};

export const submitAnswer = async (roomCode: string, userId: number, questionId: number, answer: string, timeTaken: number) => {
    try {
        await connection.invoke('SubmitAnswer', roomCode, userId, questionId, answer, timeTaken);
    } catch (err) {
        console.error('Error submitting answer:', err);
    }
};

// Event handlers
connection.on('PlayerJoined', (player) => {
    // Handle player joined event
});

connection.on('PlayerLeft', (playerId) => {
    // Handle player left event
});

connection.on('GameStarted', (gameState) => {
    // Handle game started event
});

connection.on('QuestionUpdated', (question) => {
    // Handle question update
});

connection.on('LeaderboardUpdated', (leaderboard) => {
    // Handle leaderboard update
});

connection.on('GameEnded', (results) => {
    // Handle game end
});

export default connection;
```

## API Service (src/services/api.ts)
```typescript
import axios from 'axios';

const api = axios.create({
    baseURL: 'https://your-api-url.com/api',
});

export const createRoom = async (quizId: number, userId: number) => {
    const response = await api.post('/rooms', { quizId, userId });
    return response.data;
};

export const getActiveRooms = async () => {
    const response = await api.get('/rooms/active');
    return response.data;
};

export const getRoomDetails = async (roomCode: string) => {
    const response = await api.get(`/rooms/${roomCode}`);
    return response.data;
};

export const startGame = async (roomCode: string) => {
    const response = await api.post(`/rooms/${roomCode}/start`);
    return response.data;
};

export const getGameResults = async (roomCode: string) => {
    const response = await api.get(`/rooms/${roomCode}/results`);
    return response.data;
};
```

## Types (src/services/types.ts)
```typescript
export interface GameRoomDto {
    roomCode: string;
    roomName: string;
    quizId: number;
    hostUserId: number;
    hostUsername: string;
    questions: QuestionResponseDto[];
    participants: RoomParticipantDto[];
    status: string;
    createdAt: Date;
    startedAt: Date;
}

export interface GameStateDto {
    roomCode: string;
    currentQuestionIndex: number;
    totalQuestions: number;
    startTime: Date;
    status: string;
}

export interface QuestionResponseDto {
    questionId: number;
    quizId: number;
    content: string;
    optionA: string;
    optionB: string;
    optionC: string;
    optionD: string;
    correctOption: string;
    createdAt: Date;
}

export interface RoomParticipantDto {
    userId: number;
    joinedAt: Date;
}

export interface LeaderboardEntryDto {
    userId: number;
    score: number;
    timeTaken: number;
}

export interface GameResultsDto {
    roomId: number;
    finalLeaderboard: LeaderboardEntryDto[];
    questionResults: QuestionResultDto[];
    createdAt: Date;
}
```

## Main Components

### 1. RoomList Component (src/components/Room/RoomList.tsx)
```typescript
import React, { useEffect, useState } from 'react';
import { Box, VStack, Heading, Button, useToast } from '@chakra-ui/react';
import { getActiveRooms } from '../../services/api';
import { GameRoomDto } from '../../services/types';

const RoomList: React.FC = () => {
    const [rooms, setRooms] = useState<GameRoomDto[]>([]);
    const [loading, setLoading] = useState(true);
    const toast = useToast();

    useEffect(() => {
        fetchRooms();
    }, []);

    const fetchRooms = async () => {
        try {
            const activeRooms = await getActiveRooms();
            setRooms(activeRooms);
        } catch (error) {
            toast({
                title: 'Error',
                description: 'Failed to fetch rooms',
                status: 'error',
                duration: 3000,
            });
        } finally {
            setLoading(false);
        }
    };

    return (
        <Box p={4}>
            <Heading mb={4}>Active Rooms</Heading>
            <VStack spacing={4} align="stretch">
                {rooms.map((room) => (
                    <Box
                        key={room.roomCode}
                        p={4}
                        borderWidth="1px"
                        borderRadius="lg"
                    >
                        <Heading size="md">Room: {room.roomCode}</Heading>
                        <Button
                            mt={2}
                            colorScheme="blue"
                            onClick={() => handleJoinRoom(room.roomCode)}
                        >
                            Join Room
                        </Button>
                    </Box>
                ))}
            </VStack>
        </Box>
    );
};

export default RoomList;
```

### 2. Game Component (src/components/Game/QuestionDisplay.tsx)
```typescript
import React, { useEffect, useState } from 'react';
import { Box, VStack, Heading, Text, Button, useToast } from '@chakra-ui/react';
import { GameStateDto, QuestionResponseDto } from '../../services/types';
import { submitAnswer } from '../../services/signalR';

interface GameProps {
    roomCode: string;
    userId: number;
    gameState: GameStateDto;
    currentQuestion: QuestionResponseDto;
}

const Game: React.FC<GameProps> = ({ roomCode, userId, gameState, currentQuestion }) => {
    const [selectedAnswer, setSelectedAnswer] = useState<string>('');
    const [timeTaken, setTimeTaken] = useState<number>(0);
    const toast = useToast();

    const handleAnswerSubmit = async () => {
        if (!selectedAnswer) {
            toast({
                title: 'Error',
                description: 'Please select an answer',
                status: 'warning',
                duration: 3000,
            });
            return;
        }

        try {
            await submitAnswer(roomCode, userId, currentQuestion.questionId, selectedAnswer, timeTaken);
        } catch (error) {
            toast({
                title: 'Error',
                description: 'Failed to submit answer',
                status: 'error',
                duration: 3000,
            });
        }
    };

    return (
        <Box p={4}>
            <Heading mb={4}>Question {gameState.currentQuestionIndex + 1}</Heading>
            <Text mb={4}>{currentQuestion.content}</Text>
            <VStack spacing={4} align="stretch">
                {['A', 'B', 'C', 'D'].map((option) => (
                    <Button
                        key={option}
                        onClick={() => setSelectedAnswer(option)}
                        colorScheme={selectedAnswer === option ? 'blue' : 'gray'}
                    >
                        {option}: {currentQuestion[`option${option}`]}
                    </Button>
                ))}
            </VStack>
            <Button
                mt={4}
                colorScheme="green"
                onClick={handleAnswerSubmit}
                isDisabled={!selectedAnswer}
            >
                Submit Answer
            </Button>
        </Box>
    );
};

export default Game;
```

## Custom Hooks

### useRoom Hook (src/hooks/useRoom.ts)
```typescript
import { useState, useEffect } from 'react';
import { GameRoomDto } from '../services/types';
import { getRoomDetails } from '../services/api';
import { joinRoom, leaveRoom } from '../services/signalR';

export const useRoom = (roomCode: string, userId: number) => {
    const [room, setRoom] = useState<GameRoomDto | null>(null);
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        const fetchRoom = async () => {
            try {
                const roomData = await getRoomDetails(roomCode);
                setRoom(roomData);
                await joinRoom(roomCode, userId);
            } catch (error) {
                console.error('Error fetching room:', error);
            } finally {
                setLoading(false);
            }
        };

        fetchRoom();

        return () => {
            leaveRoom(roomCode, userId);
        };
    }, [roomCode, userId]);

    return { room, loading };
};
```

## UI Features to Implement

1. **Home Page**
   - List of active rooms
   - Create new room button
   - User profile information

2. **Room Lobby**
   - Room code display
   - Player list
   - Start game button (host only)
   - Chat box

3. **Game Screen**
   - Question and options display
   - Timer
   - Live leaderboard
   - Question results

4. **Final Results**
   - Overall leaderboard
   - Question statistics
   - Play again/Exit room options

## Best Practices

1. **State Management**
   - Use React Context or Redux for global state
   - Store user info and room state

2. **Error Handling**
   - Handle SignalR connection errors
   - Show user-friendly error messages
   - Auto-reconnect on disconnection

3. **Performance**
   - Use React.memo for non-rendering components
   - Optimize SignalR data transfer
   - Lazy load large components

4. **UX/UI**
   - Responsive design
   - Smooth animations
   - User feedback
   - Dark/Light mode

## Security Considerations

1. **Authentication**
   - Secure token storage
   - User authentication before room join
   - Host permission checks

2. **Data Validation**
   - Validate data before server submission
   - Handle invalid server responses

3. **Error Handling**
   - No sensitive info in error messages
   - Handle security error cases 